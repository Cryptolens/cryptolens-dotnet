using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3;
using SKGL;
using System.Security.Cryptography;

using SKM.V3.Models;
using SKM.V3.Internal;
using SKM.V3.Methods;

namespace SKM_Test
{
    [TestClass]
    public class TestSKMv3
    {
      

        [TestMethod]
        public void GetKeysTest()
        {
            var auth = AccessToken.AccessToken.ProductMethods;
            var result = ProductMethods.GetKeys(token: auth, parameters:  new GetKeysModel { ProductId= 3349 });

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            
        }

        [TestMethod]
        public void TestGetProducts()
        {
            var auth = AccessToken.AccessToken.ProductMethods;
            var result = ProductMethods.GetProducts(token: auth, parameters: new RequestModel());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);
        }
    }
}
