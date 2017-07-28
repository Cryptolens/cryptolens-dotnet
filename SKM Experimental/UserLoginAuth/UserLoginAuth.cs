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
    public class UserLoginAuth
    {
        public static (string jsonResult, string error, string getLicenseKeyToken) GetLicenseKeys(string machineCode, string token, string appName,  RSAParameters RSAPublicKey, string existingToken = null,  RSA rsa = null)
        {
            var auth = AuthMethods.CreateAuthRequest(new Scope { GetLicenseKeys = true }, appName, machineCode, AuthMethods.GetTokenId(token), rsa);

            string tokenNew = null;

            for (int i = 0; i < 30; i++)
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

            if(tokenNew == null)
            {
                return (null, "Timeout reached. The user took too long time to authorize this request.", null);
            }

            GetLicenseKeysResultLinqSign result = null;

            try
            {
                result = Helpers.HelperMethods.SendRequestToWebAPI3<GetLicenseKeysResultLinqSign>(new GetLicenseKeysModel { Sign = true}, "/User/GetLicenseKeys", tokenNew);
            } catch (Exception ex) { return (null, "Could not contact SKM: " + ex.InnerException, tokenNew); }


            if (result == null || result.Result == ResultType.Error)
            {
                return (null, "An error occurred in the method: " + result?.Message, tokenNew);
            }

            var signedData = Convert.FromBase64String(result.ActivatedMachineCodes);

            var date = BitConverter.GetBytes(result.SignDate);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(date);

            var toSign = signedData.Concat(date).ToArray();

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

            // take this out.
            var machineCodes = JsonConvert.DeserializeObject<List<String>>(System.Text.UTF8Encoding.UTF8.GetString(signedData));
            if (machineCodes?.Count != 0 && !machineCodes.Contains(machineCode))
            {
                return (null, "This machine code has not been authorized.", tokenNew);
            }


            return (JsonConvert.SerializeObject(result), null, tokenNew);
        }

      
    }
}
