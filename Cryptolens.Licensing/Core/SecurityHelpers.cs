using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace SKM.V3.Internal
{
    /// <summary>
    /// These methods require .NET Framework 4.6 or .NET Standard.
    /// </summary>
    public class SecurityMethods
    {

#if (NET46 || NETSTANDARD2_0 || NET47 || NET471)
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

#if (NETSTANDARD2_0 || NET47 || NET471)

        /// <summary>
        /// Retrieves the public key parameters from XML string. Used for compatibility with .NET Framework ToXMLString.
        /// </summary>
        public static RSAParameters FromXMLString(string xmlPublicKey)
        {
            // inspired by https://gist.github.com/Jargon64/5b172c452827e15b21882f1d76a94be4/

            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlPublicKey);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            return parameters;

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
