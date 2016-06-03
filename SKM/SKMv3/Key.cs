using SKGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SKM.V3
{
    /// <summary>
    /// Methods that perform operations on a license key. A complete list
    /// can be found here: https://serialkeymanager.com/docs/api/v3/Key
    /// </summary>
    public static class Key
    {
        /// <summary>
        /// This method will perform a key activation, similar to Activate [Web API 2]. 
        /// In contrast to key validation, key activation is not read only since it can 
        /// change license key data depending on configurations such as trial activation,
        /// etc. If trial activation is enabled, a key can be altered. Information that 
        /// is retrieved can be signed by the server to be able to keep validate keys 
        /// without Internet connection. Please keep in mind that the Feature lock can 
        /// be used to restrict the fields that can be shown in the result (fieldsToReturn). 
        /// More about this in Remarks. 
        /// https://serialkeymanager.com/docs/api/v3/Activate
        /// </summary>
        /// <param name="auth">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="BasicResult"/> or null.</returns>
        /// <remarks>
        /// The feature lock value is used to store the filedsToReturn value. If you set a certain value in the feature lock, it will be prioritized higher than the fieldsToReturn parameter.<br></br>
        /// • To compute the value of the feature lock, please use the Hide column, for those fields that you want to omit in the result above.<br></br>
        /// • If the ActivatedMachines is hidden, only the current machine code will be included(used during this particular activation). Otherwise, all machine codes will be included.
        /// </remarks>
        public static KeyInfoResult Activate(AuthDetails auth, ActivateModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/activate/", auth.Token);
        }

        /// <summary>
        /// Checks if the license key file has been modified by an unauthorized person.
        /// This method will only return true if the license key information is exactly
        /// the same as the one that was provided and signed by SKM.
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <param name="rsaPublicKey"></param>
        /// <returns></returns>
        public static bool IsLicenceseKeyGenuine(LicenseKey licenseKey, string rsaPublicKey)
        {
            if (licenseKey?.Signature != "")
            {
                var prevSignature = licenseKey.Signature;

                try
                {
                    licenseKey.Signature = "";
                    var rawResult = licenseKey.AsDictionary();
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

                    rsa.FromXmlString(rsaPublicKey);

                    byte[] signature = Convert.FromBase64String(prevSignature);

                    // the signature should not be included into the signature :)
                    System.Diagnostics.Debug.WriteLine(String.Join(",", rawResult.Select(x => x.Value)));
                    return rsa.VerifyData(HelperMethods.GetBytes(String.Join(",", rawResult.Select(x => x.Value))), "SHA256", signature);
                }
                catch { }
                finally
                {
                    licenseKey.Signature = prevSignature;
                }
            }

            return false;

        }
    }
}
