using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKGL;

namespace SKM_Test
{
    [TestClass]
    public class TestWebAPI3
    {
        [TestMethod]
        public void ExtendLicenseTest()
        {
            var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
            var auth = new AuthDetails() { Token = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd" };

            var result = SKM.ExtendLicense(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                // the license was successfully extended with 30 days.
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
