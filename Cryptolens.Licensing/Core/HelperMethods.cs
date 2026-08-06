using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
        private const int MaxDiagnosticMessageLength = 512;
        private const string TransportFailureMessage = "The Cryptolens SDK could not contact the server.";
        private const string DeserializationFailureMessage = "The Cryptolens SDK could not deserialize the server response.";
        private const string UnexpectedFailureMessage = "The Cryptolens SDK encountered an unexpected error.";

        public static IWebProxy proxy;
        private static bool notSet = false;

        /// <summary>
        /// This should be true in most cases for performance reasons.
        /// Set it to false when persistent HTTP connections cause problems in the host application.
        /// </summary>
        public static bool KeepAlive = true;

        internal static string DOMAIN = "https://api.cryptolens.io/";

        internal static string SERVER = DOMAIN + "api/";

        /// <summary>
        /// Used to send requests to Web API 3. Handled failures return a typed error when
        /// <typeparamref name="T"/> derives from <see cref="BasicResult"/> and has a public parameterless constructor; other custom result
        /// contracts retain the legacy <see langword="default"/> behavior.
        /// </summary>
        public static T SendRequestToWebAPI3<T>(RequestModel inputParameters,
                                                string typeOfAction,
                                                string token,
                                                int version = 1,
                                                int modelVersion = 1)
        {
#if KeepAliveDisabled
            if (KeepAlive)
            {
                throw new ArgumentException("Please set Helpers.KeepAlive = false when calling the library with the 'KeepAliveDisabled' flag.");
            }
#endif

            try
            {
                string server;
                NameValueCollection requestParameters = CreateRequestParameters(inputParameters, token, modelVersion, out server);

#if !KeepAliveDisabled
                if (KeepAlive)
                {
                    using (WebClient client = new WebClient())
                    {
                        TrySetUserAgent(client);
                        ConfigureClient(client);
                        return UploadAndDeserialize<T>(client, server + typeOfAction, requestParameters);
                    }
                }
#endif

                using (WebClient client = new CustomWebClient())
                {
                    ConfigureClient(client);
                    return UploadAndDeserialize<T>(client, server + typeOfAction, requestParameters);
                }
            }
            catch (Exception ex)
            {
                RethrowIfFatal(ex);
                return CreateErrorOrDefault<T>(UnexpectedFailureMessage, UnwrapReflectionException(ex));
            }
        }

        private static NameValueCollection CreateRequestParameters(RequestModel inputParameters,
                                                                    string token,
                                                                    int modelVersion,
                                                                    out string server)
        {
            Dictionary<string, object> inputParams =
                (from property in inputParameters.GetType().GetProperties() select property)
                .ToDictionary(
                    property => property.Name,
                    property => property.GetGetMethod().Invoke(inputParameters, null) == null
                        ? string.Empty
                        : property.GetGetMethod().Invoke(inputParameters, null));

            server = SERVER;
            NameValueCollection requestParameters = new NameValueCollection();

            foreach (KeyValuePair<string, object> input in inputParams)
            {
                if (input.Key == "LicenseServerUrl" && input.Value != null)
                {
                    if (!string.IsNullOrEmpty(input.Value.ToString()))
                    {
                        server = input.Value + "/api/";
                    }

                    continue;
                }

                if (input.Value == null)
                {
                    continue;
                }

                if (input.Value.GetType() == typeof(List<short>))
                {
                    requestParameters.Add(input.Key, Newtonsoft.Json.JsonConvert.SerializeObject(input.Value));
                }
                else
                {
                    requestParameters.Add(input.Key, input.Value.ToString());
                }
            }

            requestParameters.Add("token", token);
            requestParameters.Add("v", "1");
            requestParameters.Add("modelversion", modelVersion.ToString());

            return requestParameters;
        }

        private static void ConfigureClient(WebClient client)
        {
            client.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = WebRequest.DefaultWebProxy;

            if (client.Proxy != null)
            {
                client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }
        }

        private static void TrySetUserAgent(WebClient client)
        {
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location);
                client.Headers.Add(HttpRequestHeader.UserAgent, assemblyName.Name + "/" + assemblyName.Version);
            }
            catch (Exception ex)
            {
                RethrowIfFatal(ex);
            }
        }

        private static T UploadAndDeserialize<T>(WebClient client,
                                                  string requestUri,
                                                  NameValueCollection requestParameters)
        {
            try
            {
                byte[] responseBytes = client.UploadValues(requestUri, "POST", requestParameters);
                string responseBody = Encoding.UTF8.GetString(responseBytes);
                return DeserializeOrError<T>(responseBody);
            }
            catch (WebException ex)
            {
                RethrowIfFatal(ex);
                return HandleWebException<T>(ex);
            }

        }

        private static T HandleWebException<T>(WebException exception)
        {
            if (exception.Response == null)
            {
                return CreateErrorOrDefault<T>(TransportFailureMessage, exception);
            }

            try
            {
                string responseBody;
                using (WebResponse response = exception.Response)
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return CreateErrorOrDefault<T>(TransportFailureMessage, exception);
                    }

                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }

                if (string.IsNullOrEmpty(responseBody) || responseBody.Trim().Length == 0)
                {
                    return CreateErrorOrDefault<T>(TransportFailureMessage, exception);
                }

                return DeserializeOrError<T>(responseBody);
            }
            catch (Exception ex)
            {
                RethrowIfFatal(ex);
                return CreateErrorOrDefault<T>(TransportFailureMessage, ex);
            }
        }

        private static T DeserializeOrError<T>(string responseBody)
        {
            try
            {
                T response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseBody);
                if (object.ReferenceEquals(response, null))
                {
                    return CreateErrorOrDefault<T>(
                        DeserializationFailureMessage,
                        new Newtonsoft.Json.JsonSerializationException("The server returned an empty or JSON null response."));
                }

                return response;
            }
            catch (Exception ex)
            {
                RethrowIfFatal(ex);
                return CreateErrorOrDefault<T>(DeserializationFailureMessage, ex);
            }
        }

        private static T CreateErrorOrDefault<T>(string prefix,
                                                 Exception exception)
        {
            if (!typeof(BasicResult).IsAssignableFrom(typeof(T)))
            {
                return default(T);
            }

            try
            {
                BasicResult basicResult = Activator.CreateInstance(typeof(T)) as BasicResult;
                if (basicResult == null)
                {
                    return default(T);
                }


                basicResult.Result = ResultType.Error;
                basicResult.Message = FormatDiagnosticMessage(prefix, exception);
                return (T)(object)basicResult;
            }
            catch (Exception ex)
            {
                RethrowIfFatal(ex);
                return default(T);
            }
        }

        private static string FormatDiagnosticMessage(string prefix, Exception exception)
        {
            StringBuilder builder = new StringBuilder(prefix);

            if (exception != null)
            {
                builder.Append(' ');
                builder.Append(exception.GetType().Name);

                string exceptionMessage = NormalizeWhitespace(exception.Message);
                if (!string.IsNullOrEmpty(exceptionMessage))
                {
                    builder.Append(": ");
                    builder.Append(exceptionMessage);
                }
            }

            string message = NormalizeWhitespace(builder.ToString());
            if (message.Length <= MaxDiagnosticMessageLength)
            {
                return message;
            }

            return message.Substring(0, MaxDiagnosticMessageLength - 3).TrimEnd() + "...";
        }

        private static string NormalizeWhitespace(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(value.Length);
            bool previousWasWhitespace = false;

            foreach (char character in value)
            {
                bool isWhitespace = char.IsWhiteSpace(character) || char.IsControl(character);
                if (isWhitespace)
                {
                    if (builder.Length > 0 && !previousWasWhitespace)
                    {
                        builder.Append(' ');
                    }

                    previousWasWhitespace = true;
                    continue;
                }

                builder.Append(character);
                previousWasWhitespace = false;
            }

            return builder.ToString().Trim();
        }

        private static Exception UnwrapReflectionException(Exception exception)
        {
            Exception current = exception;
            while (current is TargetInvocationException && current.InnerException != null)
            {
                current = current.InnerException;
            }

            return current;
        }

        private static void RethrowIfFatal(Exception exception)
        {
            Exception current = exception;
            while (current != null)
            {
                if (current is OutOfMemoryException ||
                    current is StackOverflowException ||
                    current is AccessViolationException)
                {
                    throw exception;
                }

                current = current.InnerException;
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
