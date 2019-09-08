using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Models;
using SKM.V3.Methods;

namespace SKM_Test
{
    [TestClass]
    public class TestSubscription
    {
        [TestMethod]
        public void TestRecordUsage()
        {
            var auth = AccessToken.AccessToken.SubscriptionMethods;
            var res = Subscription.RecordUsage(auth, new RecordUsageModel { Amount = 1, ProductId = 3349, Key = "CMXKC-GUQRW-EJUGS-RRPUR" });

            Assert.IsTrue(res != null && res.Result == ResultType.Success);
        }
    }
}
