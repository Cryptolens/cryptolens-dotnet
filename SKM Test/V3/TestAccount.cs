using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Accounts;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class TestAccount
    {
        [TestMethod]
        public void UserLoginAuthTest()
        {
            var RSAKey = "<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var res = UserAccount.GetLicenseKeys("test", "WyIxODM2IiwieHI3aVg4cFdpamZmcXRhMUF4THEwUE1FenV5UHl4TVh0bHF0amdsNiJd", "Test App", 10, RSAKey);


            Assert.IsTrue(res.Licenses.Count >= 0);
            Assert.IsTrue(res.LicenseKeyToken != "");
        }
    }
}
