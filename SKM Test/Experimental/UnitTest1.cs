using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3.Models;
using SKM.V3;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

using SKM.V3;
using SKM.V3.Methods;

namespace SKM_Test.Experimental
{
    [TestClass]
    public class PaymentFormTests
    {
        [TestMethod]
        public void PaymentFormCreateSessionTest()
        {
            var result = PaymentForm.CreateSession("WyI0NzgiLCJtSEdCQUtqTDhIWjdZVGNlcHN4OUlaRDRmaG40QzQvNDM3MGorUGpYIl0=",
                new CreateSessionModel { Expires = 80000, PaymentFormId = 3, Price = 100, Currency = "SEK", Heading = "Artem", ProductName = "product name" });


        }


    }


}
