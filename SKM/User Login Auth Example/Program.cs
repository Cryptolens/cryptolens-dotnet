using Cryptolens.SKM.Auth;
using Newtonsoft.Json;
using SKM.V3;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace User_Login_Auth_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var aaa = new RSACryptoServiceProvider(2048);
            aaa.FromXmlString("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

            //WyI0NzUiLCJ5aGQvNC9xNU80eEhyWld1UGZQN3d6TytMM0dUV2xrT1VBUlBKY3d6Il0=
            var q = UserLoginAuth.GetLicenseKeys("test123", "WyI0NzUiLCJ5aGQvNC9xNU80eEhyWld1UGZQN3d6TytMM0dUV2xrT1VBUlBKY3d6Il0=", "Artem", 5, aaa.ExportParameters(false), null, new RSACryptoServiceProvider(2048));

            //var q2 = UserLoginAuth.GetLicenseKeys("test123", "WyI0MzAiLCJvYmdWdk00WWlldXNWakdqakovTHhzRDFNenU2WklZbGU4NVBQYzEwIl0=", "Artem", aaa.ExportParameters(false), q.getLicenseKeyToken , new RSACryptoServiceProvider(2048));

            var b = JsonConvert.DeserializeObject<GetLicenseKeysResult>(q.jsonResult);
        }
    }


    public class GetLicenseKeysResult : BasicResult
    {
        public string Results { get; set; }
        public string ActivatedMachineCodes { get; set; }
        public string Signature { get; set; }

    }
}
