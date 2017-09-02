using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cryptolens.SKM.PaymentForms;
using Cryptolens.SKM.Models;

namespace SKM_Net_Core_Test
{
    [TestClass]
    public class PaymentFormTests
    {
        [TestMethod]
        public void PaymentFormCreateSessionTest()
        {
            var result = PaymentForm.CreateSession(new CreateSessionModel { Expires = 80000, PaymentFormId = 3, Price = 100, Currency = "SEK", Heading = "Artem", ProductName = "product name", CustomField = "test field" },
                "");


        }


    }
}
