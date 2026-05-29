using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class TestUsageAnalytics
    {
        private const int Limit = 1;

        [TestMethod]
        public void GetDailyAggregatesTest()
        {
            var result = AI.GetDailyAggregates(AccessToken.AccessToken.UsageAnalytics, CreateRequest());

            AssertSuccess(result);
            AssertLimitedCollection(result.Aggregates, "Aggregates");

            if (result.Aggregates.Count > 0)
            {
                Assert.IsTrue(result.Aggregates[0].Id >= 0);
                Assert.IsTrue(result.Aggregates[0].ProductId >= 0);
                Assert.IsFalse(string.IsNullOrEmpty(result.Aggregates[0].Date));
            }
        }

        [TestMethod]
        public void GetDailyCountryAggregatesTest()
        {
            var result = AI.GetDailyCountryAggregates(AccessToken.AccessToken.UsageAnalytics, CreateRequest());

            AssertSuccess(result);
            AssertLimitedCollection(result.Aggregates, "Aggregates");

            if (result.Aggregates.Count > 0)
            {
                Assert.IsTrue(result.Aggregates[0].Id >= 0);
                Assert.IsTrue(result.Aggregates[0].ProductId >= 0);
                Assert.IsFalse(string.IsNullOrEmpty(result.Aggregates[0].Date));
                Assert.IsFalse(string.IsNullOrEmpty(result.Aggregates[0].CountryCode));
            }
        }

        [TestMethod]
        public void GetKeyUsageSummariesTest()
        {
            var result = AI.GetKeyUsageSummaries(AccessToken.AccessToken.UsageAnalytics, CreateRequest());

            AssertSuccess(result);
            AssertLimitedCollection(result.Summaries, "Summaries");

            if (result.Summaries.Count > 0)
            {
                Assert.IsTrue(result.Summaries[0].Id >= 0);
                Assert.IsTrue(result.Summaries[0].ProductId >= 0);
                Assert.IsTrue(result.Summaries[0].KeyId >= 0);
            }
        }

        [TestMethod]
        public void GetKeyDevicesTest()
        {
            var result = AI.GetKeyDevices(AccessToken.AccessToken.UsageAnalytics, CreateRequest());

            AssertSuccess(result);
            AssertLimitedCollection(result.Devices, "Devices");

            if (result.Devices.Count > 0)
            {
                Assert.IsTrue(result.Devices[0].Id >= 0);
                Assert.IsTrue(result.Devices[0].ProductId >= 0);
                Assert.IsTrue(result.Devices[0].KeyId >= 0);
                Assert.IsFalse(string.IsNullOrEmpty(result.Devices[0].MachineCode));
            }
        }

        [TestMethod]
        public void GetLicenseActivityBucketsTest()
        {
            var result = AI.GetLicenseActivityBuckets(AccessToken.AccessToken.UsageAnalytics, CreateRequest());

            AssertSuccess(result);
            AssertLimitedCollection(result.Buckets, "Buckets");

            if (result.Buckets.Count > 0)
            {
                Assert.IsTrue(result.Buckets[0].Id >= 0);
                Assert.IsTrue(result.Buckets[0].ProductId >= 0);
                Assert.IsTrue(result.Buckets[0].KeyId >= 0);
                Assert.IsFalse(string.IsNullOrEmpty(result.Buckets[0].MachineCode));
            }
        }

        private static GetUsageAnalyticsModel CreateRequest()
        {
            return new GetUsageAnalyticsModel { Limit = Limit };
        }

        private static void AssertSuccess(BasicResult result)
        {
            Assert.IsNotNull(result, "The API call returned null.");

            if (result.Result != ResultType.Success)
            {
                Assert.Fail(string.IsNullOrEmpty(result.Message) ? "The API call returned an error." : result.Message);
            }
        }

        private static void AssertLimitedCollection<T>(IList<T> items, string name)
        {
            Assert.IsNotNull(items, name + " was null.");
            Assert.IsTrue(items.Count <= Limit, name + " returned more rows than requested.");
        }
    }
}
