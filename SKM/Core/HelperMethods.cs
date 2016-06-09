using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace SKM.V3.Internal
{
    /// <summary>
    /// The methods that are being used under the hood.
    /// </summary>
    public class HelperMethods
    {
        /// <summary>
        /// Used to send requests to Web API 3.
        /// </summary>
        public static T SendRequestToWebAPI3<T>(object inputParameters, 
                                                string typeOfAction,
                                                string token,
                                                WebProxy proxy = null,
                                                int version = 1 )                                                    
        {

            // converting the input
            Dictionary<string, string> inputParams = (from x in inputParameters.GetType().GetProperties() select x)
                                                          .ToDictionary(x => x.Name, x => (x.GetGetMethod()
                                                          .Invoke(inputParameters, null) == null ? "" : x.GetGetMethod()
                                                          .Invoke(inputParameters, null).ToString()));


            using (WebClient client = new WebClient())
            {
                NameValueCollection reqparm = new NameValueCollection();

                foreach (var input in inputParams)
                {
                    reqparm.Add(input.Key, input.Value);
                }

                reqparm.Add("token", token);

                //// version 1 is default so no need to send it twice.
                //if (version > 1)
                //    reqparm.Add("v", version.ToString());

                // in case we have a proxy server. if not, we set it to null to avoid unnecessary time delays.
                // based on http://stackoverflow.com/a/4420429/1275924 and http://stackoverflow.com/a/6990291/1275924. 
                client.Proxy = proxy;

                try
                {
                    byte[] responsebytes = client.UploadValues("https://serialkeymanager.com/api/" + typeOfAction, "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responsebody);
                }
                catch(Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("An error occurred when we tried to contact SKM. The following error was received: " + ex.Message);
#endif
                    return default(T);
                }

            }
        }

     
        /// <summary>
        /// Useful snippets by @Mehrdad
        /// http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
        /// </summary>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Useful snippets by @Mehrdad
        /// http://stackoverflow.com/questions/472906/converting-a-string-to-byte-array
        /// </summary>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


    }
}
