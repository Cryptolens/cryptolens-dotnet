using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        public void LoginTest()
        {
            //var result = CustomerMethods.AddCustomer(AccessToken.AccessToken.CustomerAddRemoveAccessToken,
            //                                  new AddCustomerModel() { Name = "Bob" });

            var result1 = UserAuth.Register(AccessToken.AccessToken.UserAuthAdmin, new RegisterUserModel { UserName = "test1", Password = "verysecurepassword" });

            if (!Helpers.IsSuccessful(result1))
            {
                // error
                Assert.Fail();
            }

            var result = UserAuth.Login(AccessToken.AccessToken.UserAuthAdmin, new LoginUserModel { UserName = "test1", Password = "verysecurepassword" });


            if(!Helpers.IsSuccessful(result)) 
            {
                // error
                Assert.Fail();

            }

            var passchange = UserAuth.ChangePassword(AccessToken.AccessToken.UserAuthAdmin, new ChangePasswordModel { UserName = "test1", OldPassword = "verysecurepassword", NewPassword ="test" });


            if (!Helpers.IsSuccessful(passchange))
            {
                // error
                Assert.Fail();

            }


            var log = UserAuth.Login(AccessToken.AccessToken.UserAuthAdmin, new LoginUserModel { UserName = "test1", Password="test" });


            if (!Helpers.IsSuccessful(log))
            {
                // error
                Assert.Fail();

            }




            var remove = UserAuth.RemoveUser(AccessToken.AccessToken.UserAuthAdmin, new RemoveUserModel { UserName = "test1"});


            if (!Helpers.IsSuccessful(result))
            {
                // error
                Assert.Fail();

            }



        }

    }
}
    