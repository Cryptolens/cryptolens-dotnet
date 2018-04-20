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
        public static string SERVER = "https://serialkeymanager.com/";


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
                var jObj = JObject.FromObject(inputParameters);
                //jObj.Add("token", token);
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
