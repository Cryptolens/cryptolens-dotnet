using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKGL;
namespace SKM_Test
{
    /// <summary>
    /// These tests do not access the Web API.
    /// </summary>
    [TestClass]
    [DeploymentItem("Samples\\crypto.txt")]
    public class TestExtensionMethods
    {
        [TestMethod]
        public void HasExpiredTest()
        {
            var keyInfo = new KeyInformation() 
            { 
                ExpirationDate = DateTime.Today.AddDays(1)
            };
            
          
            Assert.IsTrue(keyInfo.HasNotExpired()
                                 .IsValid());


            keyInfo.ExpirationDate = DateTime.Today;

            Assert.IsTrue( keyInfo.HasNotExpired()
                                  .IsValid());


            keyInfo.ExpirationDate = DateTime.Today.AddDays(-1);

            Assert.IsTrue(!keyInfo.HasNotExpired()
                                  .IsValid());

        }

        [TestMethod]
        public void HasValidSignature()
        {
            var ki = SKGL.SKM.LoadKeyInformationFromFile("crypto.txt");

            string rsaPubKey = "<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            Assert.IsTrue(ki.HasValidSignature(rsaPubKey)
                            .IsValid());

            Assert.IsTrue(ki.IsValid(rsaPubKey));
        }

        [TestMethod]
        public void SaveAndLoadFromFile()
        {
            var ki = new KeyInformation()
            {
                CreationDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddDays(3)
            };

            ki.SaveToFile("test123.txt");

            ki = null;

            Assert.IsTrue(ki.LoadFromFile("test123.txt")
                            .HasNotExpired()
                            .IsValid());
        }


    }
}
