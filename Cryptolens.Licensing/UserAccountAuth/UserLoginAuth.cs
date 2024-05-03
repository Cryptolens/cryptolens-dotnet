﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

using System.Security.Cryptography;
using System.Linq;

using SKM.V3.Models;
using SKM.V3.Methods;
using SKM.V3.Internal;

#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System.Text.Json;
#endif

namespace SKM.V3.Accounts
{
    /// <summary>
    /// User Account Authentication class takes care of all the steps necessary to retrieve
    /// customer licenses. Note, you need to make sure that the customer has an existing
    /// SKM account that has been associated with the customer object in your account.
    /// </summary>
    public class UserAccount
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
        /// <param name="tokenExpires">Sets the number of days the token should be valid.</param>
        /// <param name="RSAPublicKey">The RSA public key can be found here:
        /// https://app.cryptolens.io/User/Security </param>
        /// <param name="existingToken">If you have already called this method once
        /// and received a token as a result (despite an error), you can enter it here
        /// to avoid duplicate authorization by the user.</param>
        public static GetLicenseKeysResult GetLicenseKeys(string machineCode, string token, string appName, int tokenExpires, string RSAPublicKey, string existingToken = null)
        {
            string tokenNew = existingToken;

            if (string.IsNullOrEmpty(existingToken))
            {
                var auth = AuthMethods.CreateAuthRequest(new Scope { GetLicenseKeys = true }, appName, machineCode,  AuthMethods.GetTokenId(token), tokenExpires);

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
                    return new GetLicenseKeysResult { Error = "Timeout reached. The user took too long time to authorize this request." };
                }
            }

            GetLicenseKeysResultLinqSign result = null;

            try
            {
                result = HelperMethods.SendRequestToWebAPI3<GetLicenseKeysResultLinqSign>(new GetLicenseKeysModel { Sign = true, MachineCode = machineCode }, "/User/GetLicenseKeys", tokenNew);
            }
            catch (Exception ex)
            {
                return new GetLicenseKeysResult { LicenseKeyToken=tokenNew, Error = "Could not contact SKM: " + ex.InnerException };
            }


            if (result == null || result.Result == ResultType.Error)
            {
                return new GetLicenseKeysResult { LicenseKeyToken = tokenNew, Error = "An error occurred in the method: " + result?.Message };
            }


            var licenseKeys = Convert.FromBase64String(result.Results);
            var activatedMachines = Convert.FromBase64String(result.ActivatedMachineCodes);

            var date = BitConverter.GetBytes(result.SignDate);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var toSign = licenseKeys.Concat(activatedMachines.Concat(date)).ToArray();

            // only if sign enabled.
#if NET40 || NET46 || NET35 || NET45
            using (var rsaVal = new RSACryptoServiceProvider())
            {
                rsaVal.FromXmlString(RSAPublicKey);

                if (!rsaVal.VerifyData(toSign, "SHA256", Convert.FromBase64String(result.Signature)))
                {
                    // verification failed.
                    return new GetLicenseKeysResult { LicenseKeyToken = tokenNew, Error = "Verification of the signature failed." };
                }
            }
#else
            using (var rsaVal = RSA.Create())
            {
                rsaVal.ImportParameters(SecurityMethods.FromXMLString(RSAPublicKey));

                if (!rsaVal.VerifyData(toSign, Convert.FromBase64String(result.Signature),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    // verification failed.
                    return new GetLicenseKeysResult { LicenseKeyToken = tokenNew, Error = "Verification of the signature failed." };
                }
            }
#endif

#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            var machineCodes = System.Text.Json.JsonSerializer.Deserialize<List<String>>(System.Text.UTF8Encoding.UTF8.GetString(activatedMachines));
#else
            var machineCodes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<String>>(System.Text.UTF8Encoding.UTF8.GetString(activatedMachines));
#endif
            if (machineCodes?.Count != 0 && !machineCodes.Contains(machineCode))
            {
                return new GetLicenseKeysResult { LicenseKeyToken = tokenNew, Error = "This machine code has not been authorized." };
            }


            return new GetLicenseKeysResult
            {
#if NET48 || NET47_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                Licenses = JsonSerializer.Deserialize<List<KeyInfoResult>>(System.Text.UTF8Encoding.UTF8.GetString((licenseKeys))).Select(x => x.LicenseKey).ToList(),
#else
                Licenses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyInfoResult>>(System.Text.UTF8Encoding.UTF8.GetString((licenseKeys))).Select(x=> x.LicenseKey).ToList(),
#endif
                LicenseKeyToken = tokenNew
            };

        }
      
    }
}
