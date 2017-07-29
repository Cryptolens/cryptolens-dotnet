using System;
using System.Collections.Generic;
using System.Text;

using Cryptolens.SKM.Models;
using System.Threading;

using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Linq;

namespace Cryptolens.SKM.Auth
{
    /// <summary>
    /// User Login Authentication class takes care of all the steps necessary to retrieve
    /// customer licenses. Note, you need to make sure that the customer has an existing
    /// SKM account that has been associated with the customer object in your account.
    /// </summary>
    public class UserLoginAuth
    {

        /// <summary>
        /// This method will access the license keys that belong to the user by asking them to
        /// login into their account and authorize this application to get a token that will
        /// return them. In rare cases, we may get a successful authorization but not successful
        /// retrieval of license keys. In this case, the value tuple that is returned by this
        /// method will contain a token (Item3/licenseKeyToken). Next time you try to repeat
        /// the attempt, pass in the returned token into "existingToken" parameter.
        /// </summary>
        /// <param name="machineCode">The machine code you want to authorize.</param>
        /// <param name="token">The token to access the "GetToken" method.</param>
        /// <param name="appName">A user friendly name of your application.</param>
        /// <param name="RSAPublicKey">The RSA public key can be found here:
        /// https://serialkeymanager.com/User/Security </param>
        /// <param name="existingToken">If you have already called this method once
        /// and received a token as a result (despite an error), you can enter it here
        /// to avoid duplicate authorization by the user.</param>
        /// <param name="rsa">This value should either be set to "new RSACryptoServiceProvider(2048);"
        /// if you target .NET Framework. Otherwise, eg. when targeting .NET Core, it should be null.
        /// </param>
        /// <returns>A tuple containing (jsonResult, error, licenseKeyToken)</returns>
        public static (string jsonResult, string error, string licenseKeyToken) GetLicenseKeys(string machineCode, string token, string appName, RSAParameters RSAPublicKey, string existingToken = null, RSA rsa = null)
        {
            string tokenNew = existingToken;

            if (existingToken == null)
            {
                var auth = AuthMethods.CreateAuthRequest(new Scope { GetLicenseKeys = true }, appName, machineCode, AuthMethods.GetTokenId(token), rsa);

                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        tokenNew = AuthMethods.GetToken(auth, token);
                    }
                    catch (Exception ex) { }

                    if (tokenNew != null)
                        break;

                    Thread.Sleep(3000);
                }

                if (tokenNew == null)
                {
                    return (null, "Timeout reached. The user took too long time to authorize this request.", null);
                }
            }

            GetLicenseKeysResultLinqSign result = null;

            try
            {
                result = Helpers.HelperMethods.SendRequestToWebAPI3<GetLicenseKeysResultLinqSign>(new GetLicenseKeysModel { Sign = true, MachineCode = machineCode }, "/User/GetLicenseKeys", tokenNew);
            }
            catch (Exception ex) { return (null, "Could not contact SKM: " + ex.InnerException, tokenNew); }


            if (result == null || result.Result == ResultType.Error)
            {
                return (null, "An error occurred in the method: " + result?.Message, tokenNew);
            }


            var licenseKeys = Convert.FromBase64String(result.Results);
            var activatedMachines = Convert.FromBase64String(result.ActivatedMachineCodes);

            var date = BitConverter.GetBytes(result.SignDate);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var toSign = licenseKeys.Concat(activatedMachines.Concat(date)).ToArray();

            // only if sign enabled.
            using (var rsaVal = RSA.Create())
            {
                rsaVal.ImportParameters(RSAPublicKey);

                if (!rsaVal.VerifyData(toSign, Convert.FromBase64String(result.Signature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    // verification failed.
                    return (null, "Verification of the signature failed.", tokenNew);
                }

            }

            var machineCodes = JsonConvert.DeserializeObject<List<String>>(System.Text.UTF8Encoding.UTF8.GetString(activatedMachines));
            if (machineCodes?.Count != 0 && !machineCodes.Contains(machineCode))
            {
                return (null, "This machine code has not been authorized.", tokenNew);
            }


            return (JsonConvert.SerializeObject(result), null, tokenNew);
        }
      
    }
}
