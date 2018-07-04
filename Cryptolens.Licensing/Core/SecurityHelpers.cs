using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SKM.V3.Internal
{
    /// <summary>
    /// These methods require .NET Framework 4.6
    /// </summary>
    public class SecurityMethods
    {

#if (NET46 || NETSTANDARD2_0)
        public static VerificationResult VerifyObject(string signature, RSA rsa)
        {
            if (string.IsNullOrEmpty(signature))
                return null;

            var obj = JsonConvert.DeserializeObject<SignatureV1>(UTF8Encoding.UTF8.GetString(Convert.FromBase64String(signature)));

            // should return data instead.

            var dataBytes = Convert.FromBase64String(obj.Data);
            var sigBytes = Convert.FromBase64String(obj.Signature);

            if (!rsa.VerifyData(dataBytes, sigBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1))
                return null;

            return new VerificationResult { Object = UTF8Encoding.UTF8.GetString(dataBytes), SignDate = DateTimeOffset.FromUnixTimeSeconds(obj.Date).Date };
        }
#endif
    }

    public class SignatureV1
    {
        public string Data { get; set; }

        public string Signature { get; set; }

        public long Date { get; set; }

        public int Version { get; set; }
    }

    public class VerificationResult
    {
        public string Object { get; set; }

        public DateTime SignDate { get; set; }
    }
}
