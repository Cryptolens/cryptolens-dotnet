using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3;
using SKGL;
using System.Security.Cryptography;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM_Test
{
    [TestClass]
    public class TestSKMv3
    {
      

        [TestMethod]
        public void GetKeysTest()
        {
            var auth = "WyIxMjkiLCIxRVNxYStKRTloUGorQytSMndHclNBTno5dzA0Tjl1dGgvS2k5UkxHIl0=";
            var result = Product.GetKeys(token: auth, parameters:  new GetKeysModel { ProductId= 3349 });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            
        }


    }
}
