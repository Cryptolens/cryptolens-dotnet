
using SKM.V3.Internal;
using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM.V3
{
    /// <summary>
    /// A Data Object used to store information.
    /// </summary>
    public class DataObject
    {
        public long Id { get; set; }

        /// <summary>
        /// A way to identify the current object, for instance, OS_Version.
        /// </summary>
        /// <remarks>Max 10 chars.</remarks>
        public string Name { get; set; }


        private string stringValue;
        /// <summary>
        /// A string value (text) to store. Max 100 characters.
        /// </summary>
        /// <remarks>Do not assign any values to this property.
        /// Instead, please use <see cref="SetStringValue(string, string)"/></remarks>
        public string StringValue { get; set; }

        /// <summary>
        /// An int value (int32) to store.
        /// </summary>
        /// <remarks>Do not assign any values to this property.
        /// Instead, please use <see cref="SetIntValue(string, int)"/></remarks>
        public int IntValue { get; set; }

        public override string ToString()
        {
            if (this == null)
            {
                return base.ToString();
            }
            else
            {
                return Id.ToString() + "," + Name + "," + StringValue + "," + IntValue;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var dobj = (DataObject)obj;

            if (dobj.Id != Id ||
                dobj.IntValue != IntValue ||
                dobj.StringValue != StringValue ||
                dobj.Name != Name)
                return false;
            return true;
        }

        /// <summary>
        /// Sets the <see cref="IntValue"/> to a new value (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="value">The new int value</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetIntValue">https://serialkeymanager.com/docs/api/v3/SetIntValue</a> <br/>
        /// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public bool SetIntValue(string token, int value)
        {
            var parameters = new ChangeIntValueModel
            {
                Id = Id,
                IntValue = value
            };

            var result = Data.SetIntValue(token, parameters);

            if(result != null && result.Result == ResultType.Success)
            {
                IntValue = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the <see cref="IntValue"/> to a new value (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="value">The new int value</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetIntValue">https://serialkeymanager.com/docs/api/v3/SetIntValue</a> <br/>
        /// Note also: Integer overflows are not allowed. If you attempt to assign an int value that is beyond the limits of an int32, zero will be assigned to the data object's IntValue.</remarks>
        /// <returns>Returns <see cref="ListOfDataObjectsResult"/> or null.</returns>
        public bool SetIntValue(string token, int value, LicenseKey licenseKey)
        {
            var parameters = new ChangeIntValueToKeyModel
            {
                ProductId = licenseKey.ProductId,
                Key = licenseKey.Key,
                Id = Id,
                IntValue = value
            };

            var result = Data.SetIntValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                IntValue = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the <see cref="StringValue"/> to a new value (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="value">The new int value</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetStringValue">https://serialkeymanager.com/docs/api/v3/SetStringValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool SetStringValue(string token, string value)
        {
            var parameters = new ChangeStringValueModel
            {
                Id=Id,
                StringValue = value
            };

            var result = Data.SetStringValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                StringValue = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the <see cref="StringValue"/> to a new value (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="value">The new int value</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/SetStringValue">https://serialkeymanager.com/docs/api/v3/SetStringValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool SetStringValue(string token, string value, LicenseKey licenseKey)
        {
            var parameters = new ChangeStringValueToKeyModel
            {
                ProductId = licenseKey.ProductId,
                Key = licenseKey.Key,
                Id = Id,
                StringValue = value
            };

            var result = Data.SetStringValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                StringValue = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increments the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="incrementValue">The number we should increment by.</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool IncrementIntValue(string token, int incrementValue)
        {
            return IncrementIntValue(token, incrementValue, false, 0);
        }

        /// <summary>
        /// Increments the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="incrementValue">The number we should increment by.</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool IncrementIntValue(string token, int incrementValue, LicenseKey licenseKey)
        {
            return IncrementIntValue(token, incrementValue,licenseKey, false, 0);
        }

        /// <summary>
        /// Increments the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="incrementValue">The number we should increment by.</param>
        /// <param name="enableBound">
        /// If set to true, it will be possible to specify an upper/lower bound. 
        /// (for Increment Int Value) For example, if you set the <paramref name="upperBound"/> parameter (below) to 10, you
        /// will be able to increment the int value until you reach ten (inclusive).
        /// Once the upper bound is reached, an error will be thrown.
        /// </param>
        /// <param name="upperBound">The upper bound. It only works if <paramref name="enableBound"/> is set to true.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool IncrementIntValue(string token, int incrementValue, bool enableBound =false, int upperBound = 0)
        {
            var parameters = new ChangeIntValueModel
            {
                Id = Id,
                IntValue = incrementValue,
                EnableBound = enableBound,
                Bound = upperBound
            };

            var result = Data.IncrementIntValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                IntValue += incrementValue;
                return true;
            }
            return false;
        }



        /// <summary>
        /// Increments the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="incrementValue">The number we should increment by.</param>
        /// <param name="enableBound">
        /// If set to true, it will be possible to specify an upper/lower bound. 
        /// (for Increment Int Value) For example, if you set the <paramref name="upperBound"/> parameter (below) to 10, you
        /// will be able to increment the int value until you reach ten (inclusive).
        /// Once the upper bound is reached, an error will be thrown.
        /// </param>
        /// <param name="upperBound">The upper bound. It only works if <paramref name="enableBound"/> is set to true.</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool IncrementIntValue(string token, int incrementValue, LicenseKey licenseKey, bool enableBound = false, int upperBound = 0)
        {
            var parameters = new ChangeIntValueToKeyModel
            {
                Id = Id,
                IntValue = incrementValue,
                EnableBound = enableBound,
                Bound = upperBound,
                ProductId = licenseKey.ProductId,
                Key = licenseKey.Key
            };

            var result = Data.IncrementIntValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                IntValue += incrementValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Decrements the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="decrementValue">The number we should decrement by.</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool DecrementIntValue(string token, int decrementValue, LicenseKey licenseKey)
        {
            return DecrementIntValue(token, decrementValue,licenseKey, false, 0);
        }


        /// <summary>
        /// Decrements the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="decrementValue">The number we should decrement by.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool DecrementIntValue(string token, int decrementValue)
        {
            return DecrementIntValue(token, decrementValue, false, 0);
        }


        /// <summary>
        /// Decrements the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="decrementValue">The number we should decrement by.</param>
        /// <param name="enableBound">
        /// If set to true, it will be possible to specify an lower bound. 
        /// (for Decrement Int Value) For example, if you set the <paramref name="lowerBound"/> parameter (below) to 0, 
        /// you will be able to decrement the int value until you reach zero (inclusive).
        /// Once the lower bound is reached, an error will be thrown.
        /// </param>
        /// <param name="lowerBound">The upper bound. It only works if <paramref name="enableBound"/> is set to true.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool DecrementIntValue(string token, int decrementValue, bool enableBound = false, int lowerBound = 0)
        {
            var parameters = new ChangeIntValueModel
            {
                Id= Id,
                IntValue = decrementValue,
                EnableBound = enableBound,
                Bound = lowerBound
            };

            var result = Data.DecrementIntValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                IntValue -= decrementValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Decrements the <see cref="IntValue"/> by a given amount. (in SKM Platform).
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="decrementValue">The number we should decrement by.</param>
        /// <param name="enableBound">
        /// If set to true, it will be possible to specify an lower bound. 
        /// (for Decrement Int Value) For example, if you set the <paramref name="lowerBound"/> parameter (below) to 0, 
        /// you will be able to decrement the int value until you reach zero (inclusive).
        /// Once the lower bound is reached, an error will be thrown.
        /// </param>
        /// <param name="lowerBound">The upper bound. It only works if <paramref name="enableBound"/> is set to true.</param>
        /// <param name="licenseKey">The license key we should associate with this data object.</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/IncrementIntValue">https://serialkeymanager.com/docs/api/v3/IncrementIntValue</a> <br/>
        /// </remarks>
        /// <returns>Returns true if successful or false otherwise.</returns>
        public bool DecrementIntValue(string token, int decrementValue, LicenseKey licenseKey, bool enableBound = false, int lowerBound = 0)
        {
            var parameters = new ChangeIntValueToKeyModel
            {
                ProductId = licenseKey.ProductId,
                Key = licenseKey.Key,
                Id = Id,
                IntValue = decrementValue,
                EnableBound = enableBound,
                Bound = lowerBound
            };

            var result = Data.DecrementIntValue(token, parameters);

            if (result != null && result.Result == ResultType.Success)
            {
                IntValue -= decrementValue;
                return true;
            }
            return false;
        }
    }

   

    /// <summary>
    /// This class adds some more fields that tell us which referencer this data object
    /// belongs to.
    /// </summary>
    public class DataObjectWithReferencer : DataObject
    {
        /// <summary>
        /// Indicates if the data object should be added to a
        /// license key, a product or the entire user account. 
        /// <see cref="DataObject"/>
        /// </summary>
        public DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the Referencer. It can either be an id to a product
        /// that you have or to a license key. When ReferencerType is set
        /// to User, there is no need to set this value.
        /// </summary>
        public int ReferencerId { get; set; }
    }

    /// <summary>
    /// Type of referencer (association), eg. a Data object that belongs to
    /// a product, or a key or user
    /// </summary>
    public enum DataObjectType : byte
    {
        User = 0,
        Product = 1,
        Key = 2,
        MachineCode = 3
    }
}
