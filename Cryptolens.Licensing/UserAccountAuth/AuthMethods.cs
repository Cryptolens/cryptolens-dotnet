using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.Net;

using SKM.V3.Models;

namespace SKM.V3.Internal
{
    /// <summary>
    /// Methods that are used to be able to receive an access token that can list all
    /// </summary>
    public class AuthMethods
    {
        public static CreateAuthRequestResult CreateAuthRequest(Scope scope, string appName, string machineCode, int tokenId, int expires)
        {
            var authToken = new byte[30];
#if NET35
            var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(authToken);
#else
            using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(authToken);
            }
#endif


            RSAParameters RSAParamsPrivate;
            string RSAParamsPublic = "";

#if NET40 || NET46 || NET35
            var rsa = new RSACryptoServiceProvider(2048);
#else
            var rsa = RSA.Create();
            rsa.KeySize = 2048;
#endif

            RSAParamsPrivate = rsa.ExportParameters(true);
            RSAParamsPublic = JsonConvert.SerializeObject(rsa.ExportParameters(false));

            var model = new AuthorizeAppModel()
            {
                AuthorizationToken = Convert.ToBase64String(authToken),
                Expires = expires,
                PublicKey = RSAParamsPublic,
                Scope = JsonConvert.SerializeObject(scope),
                VendorAppName = appName,
                DeviceName = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME"), //Environment.MachineName in .NET Standard > 1.5,
                MachineCode = machineCode,
                TokenId = tokenId,
                Algorithm = SignatureAlgorithm.RSA_2048
            };


            OpenBrowser(HelperMethods.DOMAIN + "User/AuthorizeApp/?" + GetQueryString(model));

            return new CreateAuthRequestResult
            {
                AuthorizationToken = authToken,
                Parameters = RSAParamsPrivate
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authInfo"></param>
        /// <param name="token">A token with GetChallenge and GetToken permissions.</param>
        /// <returns></returns>
        public static string GetToken(CreateAuthRequestResult authInfo, string token, RequestModel optionalParams = null)
        {
            // 1. Get the challenge
            var initResponse = HelperMethods.SendRequestToWebAPI3<GetChallengeResult>(
                            new GetChallengeModel {
                                AuthorizationToken = Convert.ToBase64String(authInfo.AuthorizationToken), LicenseServerUrl = optionalParams?.LicenseServerUrl }, 
                                "/auth/GetChallenge", 
                                token);

            if(initResponse == null ||initResponse.Result == ResultType.Error)
            {
                return null;
            }

            var challenge = Convert.FromBase64String(initResponse.Challenge);

#if NET40 || NET35
            long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
#else
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#endif

            var date = BitConverter.GetBytes(unixTimestamp);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var toSign = challenge.Concat(date).ToArray();

            var response = new byte[] { };

#if NET40 || NET35
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(authInfo.Parameters);
            response = rsa.SignData(toSign, "SHA512");
#else
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                rsa.ImportParameters(authInfo.Parameters);
                response = rsa.SignData(toSign, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            }
#endif

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var result = HelperMethods.SendRequestToWebAPI3<GetTokenResult>(
                         new GetTokenModel {
                             AuthorizationToken = Convert.ToBase64String(authInfo.AuthorizationToken),
                             Date = BitConverter.ToInt64(date, 0),
                             SignedChallenge = Convert.ToBase64String(response),
                             LicenseServerUrl = optionalParams?.LicenseServerUrl
                         }, 
                            "/auth/GetToken/", 
                            token);


            // 2. Submit challenge and get the token
            return result.Token;
        }

        public static int GetTokenId(string token)
        {
            string jsonData = HelperMethods.DecodeFrom64(token);

            string[] data = JsonConvert.DeserializeObject<string[]>(jsonData);

            return Convert.ToInt32(data[0]);
        }

        private static void OpenBrowser(string url)
        {
            //Credits to https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/

            try
            {
                Process.Start(url);
            }
            catch
            {
#if NETSTANDARD2_0
                //hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
#else
                throw;
#endif
            }
        }

        /// <summary>
        /// Generates a query string that represents an object (url friendly). Adapted based on http://stackoverflow.com/a/6848707/1275924.
        /// </summary>
        private static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + System.Uri.EscapeDataString(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}
