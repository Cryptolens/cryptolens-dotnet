using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Methods;
using SKM.V3.Models;


namespace SKM_Test
{
    [TestClass]
    public class TestAI
    {
        [TestMethod]
        public void RegisterEventTest()
        {
            // eg. inside the client app
            var res = AI.RegisterEvent(AccessToken.AccessToken.RegisterEvent, 
                new RegisterEventModel {  FeatureName = "F1",
                    EventName = "start",
                    MachineCode = Helpers.GetMachineCode()});

            if(res == null || res.Result == ResultType.Error)
            {
                Assert.Fail(res.Message);
            }

            // eg. when the customer has bought the product.
            var res2 = AI.RegisterEvent(AccessToken.AccessToken.RegisterEvent, 
                new RegisterEventModel { FeatureName = "F1",
                    EventName = "purchase",
                    Value = 30,
                    Currency = "USD",
                    MachineCode = Helpers.GetMachineCode() });

            if (res2 == null || res2.Result == ResultType.Error)
            {
                Assert.Fail(res.Message);
            }

        }

        [TestMethod]
        public void RegisterEventsTest()
        {
            // register multiple events at the same time

            var eventsCache = new List<Event>();

            eventsCache.Add(new Event() { FeatureName = "F2", EventName = "start" });
            eventsCache.Add(new Event() { FeatureName = "F2", EventName = "closed" });

            var res = AI.RegisterEvents(AccessToken.AccessToken.RegisterEvent,
                new RegisterEventsModel { Events = eventsCache,
                    MachineCode = Helpers.GetMachineCode() });

            if (res == null || res.Result == ResultType.Error)
            {
                Assert.Fail(res.Message);
            }

        }
    }
}
