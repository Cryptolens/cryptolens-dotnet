using System;
using System.Net.Http;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.Net;

using Cryptolens.SKM.Models;
using Cryptolens.SKM.Helpers;

namespace Cryptolens.SKM.Auth
{
    /// <summary>
    /// Methods that are used to be able to receive an access token that can list all
    /// </summary>
    public class AuthMethods
    {
        public static (byte[] authorizationToken , RSAParameters parameters) CreateAuthRequest(Scope scope, string appName, string machineCode, int tokenId, RSA rsa = null)
        {
            var authToken = new byte[30];
            using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(authToken);
            }

            RSAParameters RSAParamsPrivate;
            string RSAParamsPublic = "";


            if (rsa == null)
            {
                // we are not targeting the .NET Framework.
                rsa = RSA.Create();
                rsa.KeySize = 2048;
            }

            RSAParamsPrivate = rsa.ExportParameters(true);
            RSAParamsPublic = JsonConvert.SerializeObject(rsa.ExportParameters(false));

            var model = new AuthorizeAppModel()
            {
                AuthorizationToken = Convert.ToBase64String(authToken),
                Expires = 30,
                PublicKey = RSAParamsPublic,
                Scope = JsonConvert.SerializeObject(scope),
                VendorAppName = appName,
                DeviceName = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? Environment.GetEnvironmentVariable("HOSTNAME"), //Environment.MachineName in .NET Standard > 1.5,
                MachineCode = machineCode,
                TokenId = tokenId,
                Algorithm = SignatureAlgorithm.RSA_2048
            };


            OpenBrowser(HelperMethods.SERVER + "/User/AuthorizeApp/?" + GetQueryString(model));

            return (authToken, RSAParamsPrivate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authInfo"></param>
        /// <param name="token">A token with GetChallenge and GetToken permissions.</param>
        /// <returns></returns>
        public static string GetToken((byte[] authorizationToken, RSAParameters parameters) authInfo, string token)
        {
            // 1. Get the challenge
            var initResponse = HelperMethods.SendRequestToWebAPI3<GetChallengeResult>(
                            new GetChallengeModel {
                                AuthorizationToken = Convert.ToBase64String(authInfo.authorizationToken) }, 
                                "/auth/GetChallenge", 
                                token);

            if(initResponse == null ||initResponse.Result == ResultType.Error)
            {
                return null;
            }

            var challenge = Convert.FromBase64String(initResponse.Challenge);
            var date = BitConverter.GetBytes(((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var toSign = challenge.Concat(date).ToArray();

            var response = new byte[] { };

            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                rsa.ImportParameters(authInfo.parameters);
                response = rsa.SignData(toSign, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            }


            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var result = HelperMethods.SendRequestToWebAPI3<GetTokenResult>(
                         new GetTokenModel {
                             AuthorizationToken = Convert.ToBase64String(authInfo.authorizationToken),
                             Date = BitConverter.ToInt64(date, 0),
                             SignedChallenge = Convert.ToBase64String(response)
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
            }
        }

        /// <summary>
        /// Generates a query string that represents an object (url friendly). From http://stackoverflow.com/a/6848707/1275924.
        /// </summary>
        private static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetRuntimeProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}
