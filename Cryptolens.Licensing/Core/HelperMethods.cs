using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using SKM.V3.Models;

namespace SKM.V3.Internal
{
    /// <summary>
    /// <para>
    /// This namespace contains methods that are used internally to communicate with the Web API. You will most likely not need to use them.
    /// </para>
    /// </summary>
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// The methods that are being used under the hood.
    /// </summary>
    public class HelperMethods
    {

        public static IWebProxy proxy;
        private static bool notSet = false;

        /// <summary>
        /// This should be true in most case for performance reasons.
        /// In rare cases, please set this to true.
        /// </summary>
        public static bool KeepAlive = true;



        internal static string DOMAIN = "https://app.cryptolens.io/";

        internal static string SERVER = DOMAIN + "api/";


        /// <summary>
        /// Used to send requests to Web API 3.
        /// </summary>
        public static T SendRequestToWebAPI3<T>(RequestModel inputParameters, 
                                                string typeOfAction,
                                                string token,
                                                int version = 1,
                                                int modelVersion = 1)                                                    
        {
            // converting the input
            Dictionary<string, string> inputParams = (from x in inputParameters.GetType().GetProperties() select x)
                                                          .ToDictionary(x => x.Name, x => (x.GetGetMethod()
                                                          .Invoke(inputParameters, null) == null ? "" : x.GetGetMethod()
                                                          .Invoke(inputParameters, null).ToString()));
            string server = SERVER;

            if (KeepAlive)
            {
                using (WebClient client = new WebClient())
                {
                    NameValueCollection reqparm = new NameValueCollection();

                    foreach (var input in inputParams)
                    {
                        if (input.Key == "LicenseServerUrl")
                        {
                            if (!string.IsNullOrEmpty(input.Value))
                            {
                                server = input.Value + "/api/";
                            }
                            continue;
                        }

                        reqparm.Add(input.Key, input.Value);
                    }

                    reqparm.Add("token", token);
                    reqparm.Add("v", "1");
                    reqparm.Add("modelversion", modelVersion.ToString());

                    // make sure .NET uses the default proxy set up on the client device.
                    client.Proxy = WebRequest.DefaultWebProxy;
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    try
                    {
                        byte[] responsebytes = client.UploadValues(server + typeOfAction, "POST", reqparm);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);

                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responsebody);
                    }
                    catch (WebException ex)
                    {
                        using (var sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                        }
                    }
                    catch (Exception ex)
                    {
                        return default(T);
                    }

                }
            }
            else
            {
                using (WebClient client = new CustomWebClient())
                {
                    NameValueCollection reqparm = new NameValueCollection();

                    foreach (var input in inputParams)
                    {
                        if (input.Key == "LicenseServerUrl")
                        {
                            if (!string.IsNullOrEmpty(input.Value))
                            {
                                server = input.Value + "/api/";
                            }
                            continue;
                        }

                        reqparm.Add(input.Key, input.Value);
                    }

                    reqparm.Add("token", token);
                    reqparm.Add("v", "1");
                    reqparm.Add("modelversion", modelVersion.ToString());

                    // make sure .NET uses the default proxy set up on the client device.
                    client.Proxy = WebRequest.DefaultWebProxy;
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    try
                    {
                        byte[] responsebytes = client.UploadValues(server + typeOfAction, "POST", reqparm);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);

                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responsebody);
                    }
                    catch (WebException ex)
                    {
                        using (var sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                        }
                    }
                    catch (Exception ex)
                    {
                        return default(T);
                    }

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

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }


    }

    // from https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/best-practices/business-logic/set-keepalive-false-interacting-external-hosts-plugin
    internal class CustomWebClient : WebClient
    {
        // Overrides the GetWebRequest method and sets keep alive to false
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.KeepAlive = false;

            return req;
        }
    }
}
