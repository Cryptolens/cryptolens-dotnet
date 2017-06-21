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
        public void AddCustomerTest()
        {
            var result = CustomerMethods.AddCustomer(AccessToken.AccessToken.CustomerAddRemoveAccessToken,
                                              new AddCustomerModel() { Name = "Bob" });

            if(result.Result == ResultType.Error)
            {
                Assert.Fail();
            }

            var result2 = CustomerMethods.RemoveCustomer(AccessToken.AccessToken.CustomerAddRemoveAccessToken,
                                  new RemoveCustomerModel() { CustomerId = result.CustomerId});

            if (result2.Result == ResultType.Error)
            {
                Assert.Fail();
            }
        }


        [TestMethod]
        public void GetCustomerLicensesTest()
        {
            var result = CustomerMethods.GetCustomerLicenses(AccessToken.AccessToken.GetCustomerLicenses, 
                new GetCustomerLicensesModel {  CustomerId = 3});

            if(result.Result == ResultType.Error)
            {
                Assert.Fail();
            }

        }
    }
}
    