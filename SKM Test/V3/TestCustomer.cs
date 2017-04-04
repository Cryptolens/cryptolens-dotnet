using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class TestCustomer
    {
        [TestMethod]
        public void AddCustomer()
        {
            var result = Customer.AddCustomer(AccessToken.AccessToken.CustomerAddRemoveAccessToken,
                                              new AddCustomerModel() { Name = "Bob" });

            if(result.Result == ResultType.Error)
            {
                Assert.Fail();
            }

            var result2 = Customer.RemoveCustomer(AccessToken.AccessToken.CustomerAddRemoveAccessToken,
                                  new RemoveCustomerModel() { CustomerId = result.CustomerId});

            if (result2.Result == ResultType.Error)
            {
                Assert.Fail();
            }
        }

    }
}
    