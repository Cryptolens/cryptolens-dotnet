using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SKGL
{
    /// <summary>
    /// Based on: http://stackoverflow.com/a/4944547/1275924
    /// </summary>
    internal static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<string, string> source)
            where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, string> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        /// <summary>
        /// Convert object to dictionary.
        /// </summary>
        public static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null).ToStringCultureSpecific()

            );

        }
    }

    internal static class CultureSpecificToString
    {
        /// <summary>
        /// A modified to string method that should used to get a consistent formatting
        /// for numbers, dates, and other types.
        /// 
        /// For numbers: using English culture (eg. 5.3 = 5^3 * 10^-1 and 1,000 = 1000)
        /// For dates: using Swedish
        /// </summary>
        /// <returns></returns>
        public static string ToStringCultureSpecific(this object source)
        {
            if (source is DateTime)
                return ((DateTime)source).ToString(ConfigValues.DEFAULT_TIME_REPSENTATION);
            else if (source is IEnumerable<DataObject>)
            {
                var obj = (IEnumerable<DataObject>)source;
                var sw = new StringBuilder();
                sw.Append("[");
                foreach (var item in obj)
                {
                    sw.Append(item.ToStringCultureSpecific());
                    sw.Append(",");
                }
                sw.Append("]");
                return sw.ToString();
            }
            else if (source is IEnumerable<ActivationData>)
            {
                var obj = (IEnumerable<ActivationData>)source;
                var sw = new StringBuilder();
                sw.Append("[");
                foreach (var item in obj)
                {
                    sw.Append(item.ToStringCultureSpecific());
                    sw.Append(",");
                }
                sw.Append("]");
                return sw.ToString();
            }
            return source?.ToString();
        }
    }
}
