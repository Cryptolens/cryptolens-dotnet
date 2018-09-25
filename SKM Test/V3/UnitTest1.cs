using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Models;
using SKM.V3.Methods;

namespace SKM_Test.V3
{
    [TestClass]
    public class TestMessage
    {
        [TestMethod]
        public void GetMessagesTest()
        {
            var result = Message.GetMessages(AccessToken.AccessToken.GetMessages,
                new GetMessagesModel { Channel = "stable",
                    Time = new DateTimeOffset(2018, 09, 24,0,0,0, new TimeSpan()).ToUnixTimeSeconds()});

            if(result == null && result.Result == ResultType.Error)
            {
                Assert.Fail();
            }
        }
    }
}
