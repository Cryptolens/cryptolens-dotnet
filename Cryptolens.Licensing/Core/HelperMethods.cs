using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;

using SKM.V3.Models;


#if NET48
using System.Text.Json;
#endif

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



        internal static string DOMAIN = "https://api.cryptolens.io/";

        internal static string SERVER = DOMAIN + "api/";


        /// <summary>
        /// Used to send requests to Web API 3.
        /// </summary>
        [SecuritySafeCritical]
        public static T SendRequestToWebAPI3<T>(RequestModel inputParameters, 
                                                string typeOfAction,
                                                string token,
                                                int version = 1,
                                                int modelVersion = 1)                                                    
        {
            // converting the input
            Dictionary<string, object> inputParams = (from x in inputParameters.GetType().GetProperties() select x)
                                                          .ToDictionary(x => x.Name, x => (x.GetGetMethod()
                                                          .Invoke(inputParameters, null) == null ? "" : x.GetGetMethod()
                                                          .Invoke(inputParameters, null)));
            string server = SERVER;

            if (KeepAlive)
            {
#if !KeepAliveDisabled
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        var asm = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location);
                        client.Headers.Add(HttpRequestHeader.UserAgent, $"{asm.Name}/{asm.Version}");
                    }
                    catch (Exception ex) { }

                NameValueCollection reqparm = new NameValueCollection();

                    foreach (var input in inputParams)
                    {
                        if (input.Key == "LicenseServerUrl" && input.Value != null)
                        {
                            if (!string.IsNullOrEmpty(input.Value.ToString()))
                            {
                                server = input.Value + "/api/";
                            }
                            continue;
                        }

                        if(input.Value == null) { continue; }

                        if (input.Value.GetType() == typeof(List<short>))
                        {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER

                            reqparm.Add(input.Key, System.Text.Json.JsonSerializer.Serialize(input.Value));
#else
                            reqparm.Add(input.Key, Newtonsoft.Json.JsonConvert.SerializeObject(input.Value));
#endif

                        }
                        else
                        {
                            reqparm.Add(input.Key, input.Value.ToString());
                        }
                    }

                    reqparm.Add("token", token);
                    reqparm.Add("v", "1");
                    reqparm.Add("modelversion", modelVersion.ToString());

                    // make sure .NET uses the default proxy set up on the client device.
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Proxy = WebRequest.DefaultWebProxy;
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    try
                    {
                        byte[] responsebytes = client.UploadValues(server + typeOfAction, "POST", reqparm);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);
#if NET48 || NET47_OR_GREATER  || NETSTANDARD2_0_OR_GREATER
                        return System.Text.Json.JsonSerializer.Deserialize<T>(responsebody, new System.Text.Json.JsonSerializerOptions {  PropertyNameCaseInsensitive=true });

#else
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responsebody);
#endif
                    }
                    catch (WebException ex)
                    {
                        try
                        {
                            using (var sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                            {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                                return System.Text.Json.JsonSerializer.Deserialize<T>(sr.ReadToEnd(), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
#else
                                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
#endif
                            }
                        }
                        catch (Exception ex2)
                        {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                            return System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(new BasicResult { Result = ResultType.Error, Message = "An error occurred when contacting the server." }), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
#else
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(new BasicResult { Result = ResultType.Error, Message = "An error occurred when contacting the server." }));
#endif
                        }
                    }
                    catch (Exception ex)
                    {
                        return default(T);
                    }

                }
#else
                throw new ArgumentException("Please set Helpers.KeepAlive = false when calling the library with the 'KeepAliveDisabled' flag.");
#endif
                        }
            else
            {
                using (WebClient client = new CustomWebClient())
                {
                    NameValueCollection reqparm = new NameValueCollection();

                    foreach (var input in inputParams)
                    {
                        if (input.Key == "LicenseServerUrl" && input.Value != null)
                        {
                            if (!string.IsNullOrEmpty(input.Value.ToString()))
                            {
                                server = input.Value.ToString() + "/api/";
                            }
                            continue;
                        }

                        if (input.Value == null) { continue; }

                        if (input.Value.GetType() == typeof(List<short>))
                        {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                            reqparm.Add(input.Key, System.Text.Json.JsonSerializer.Serialize(input.Value));
#else
                            reqparm.Add(input.Key, Newtonsoft.Json.JsonConvert.SerializeObject(input.Value));
#endif
                        }
                        else
                        {
                            reqparm.Add(input.Key, input.Value.ToString());
                        }
                    }

                    reqparm.Add("token", token);
                    reqparm.Add("v", "1");
                    reqparm.Add("modelversion", modelVersion.ToString());

                    // make sure .NET uses the default proxy set up on the client device.
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.Proxy = WebRequest.DefaultWebProxy;
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    try
                    {
                        byte[] responsebytes = client.UploadValues(server + typeOfAction, "POST", reqparm);
                        string responsebody = Encoding.UTF8.GetString(responsebytes);

#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER

                        return System.Text.Json.JsonSerializer.Deserialize<T>(responsebody, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

#else
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responsebody);
#endif
                    }
                    catch (WebException ex)
                    {
                        using (var sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                        {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER

                            return System.Text.Json.JsonSerializer.Deserialize<T>(sr.ReadToEnd(), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
#else
                                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
#endif
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
