using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3;
using SKGL;
using System.Security.Cryptography;

using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class TestSKMv3
    {
        [TestMethod]
        public void ActivateTest()
        {
            var auth =  "WyIxMTgiLCJkN0dEREZ0YW03alNhRVNtV3dOQkxZdjJlMWFTVlpacjNVaisxNFBZIl0=" ;
            var result = Key.Activate(token: auth , parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign=true , MachineCode="foo" });
            System.Diagnostics.Debug.WriteLine(result);


            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            Assert.IsTrue(result.LicenseKey.IsValid(TestCases.TestData.pubkey));

            result.LicenseKey.Signature = "test";

            Assert.IsFalse(result.LicenseKey.IsValid(TestCases.TestData.pubkey));
        }

        [TestMethod]
        public void SaveFileToFileTest()
        {
            var test = new LicenseKey() { Key = "abc" };
            test.SaveToFile("c:\\file.skm");
            
        }

        [TestMethod]
        public void GetKeysTest()
        {
            var auth = "WyIxMjkiLCIxRVNxYStKRTloUGorQytSMndHclNBTno5dzA0Tjl1dGgvS2k5UkxHIl0=";
            var result = Product.GetKeys(token: auth, parameters:  new GetKeysModel { ProductId= 3349 });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            
        }

        [TestMethod]
        public void SaveLoadFile()
        {
            var key = new LicenseKey() { Key = "hello", Expires = DateTime.Today};

            key.SaveToFile();

            var load = new LicenseKey().LoadFromFile();

            Assert.IsTrue(key.Key == load.Key);

        }
    }
}
