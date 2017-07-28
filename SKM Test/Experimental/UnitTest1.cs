using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3.Models;
using SKM.V3;
using System.Collections.Generic;
using Cryptolens.SKM.Auth;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace SKM_Test.Experimental
{
    [TestClass]
    public class UserLoginAuthTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var aaa = new RSACryptoServiceProvider(2048);
            aaa.FromXmlString("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

            var q = UserLoginAuth.GetLicenseKeys("test123", "WyI0MzAiLCJvYmdWdk00WWlldXNWakdqakovTHhzRDFNenU2WklZbGU4NVBQYzEwIl0=", "Artem", aaa.ExportParameters(false), null, new RSACryptoServiceProvider(2048));

            var b = JsonConvert.DeserializeObject<GetLicenseKeysResult>(q.jsonResult);
        }
    }

    public class GetLicenseKeysResult : BasicResult
    {
        public List<LicenseKey> Results { get; set; }
        public string ActivatedMachineCodes { get; set; }
        public string Signature { get; set; }

    }
}
