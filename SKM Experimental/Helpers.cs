using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Text;

using Cryptolens.SKM.Models;
using System.Security.Cryptography;

using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Cryptolens.SKM.Helpers
{
    /// <summary>
    /// The methods that are being used under the hood.
    /// </summary>
    public class HelperMethods
    {
        public static string SERVER = "https://localhost:44300/";


        /// <summary>
        /// Used to send requests to Web API 3.
        /// </summary>
        public static T SendRequestToWebAPI3<T>(object inputParameters,
                                                string typeOfAction,
                                                string token,
                                                int version = 1)
        {

            var handler = new System.Net.Http.HttpClientHandler(); // remove later.
            using (HttpClient client = new HttpClient(handler))
            {
                // malicious code starts
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    // Log it, then use the same answer it would have had if we didn't make a callback.
                    Console.WriteLine(cert);
                    return true;
                };
                // malicious code ends.

                // supported from .NET Standard 1.5

                //Dictionary<string, string> inputParams = (from x in inputParameters.GetType().GetRuntimeProperties() select x)
                //                                         .ToDictionary(x => x.Name, x => (x.GetGetMethod()
                //                                         .Invoke(inputParameters, null) == null ? "" : x.GetGetMethod()
                //                                         .Invoke(inputParameters, null).ToString()));

                //inputParams.Add("token", token);
                //inputParams.Add("v", "1");


                var jObj = JObject.FromObject(inputParameters);
                jObj.Add("token", token);
                jObj.Add("v", 1);

                var serializedContent = jObj.ToString();

                var buffer = System.Text.Encoding.UTF8.GetBytes(serializedContent);
                var httpcontent = new ByteArrayContent(buffer);

                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpcontent.Headers.Add("token", token);

                // TODO: check status code, etc.

                return JsonConvert.DeserializeObject<T>(client.PostAsync(SERVER + "/api" + typeOfAction, httpcontent).Result.Content.ReadAsStringAsync().Result);
            }
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

    }
}
