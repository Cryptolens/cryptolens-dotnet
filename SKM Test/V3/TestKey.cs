using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Models;
using System.Collections.Generic;

namespace SKM_Test
{
    [TestClass]
    public class TestKey
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void IsOnRightMachineTest()
        {
            var ki = new LicenseKey { ActivatedMachines = new List<ActivationData>() { new ActivationData {  Mid = "test"} } };

            var result = ki.IsOnRightMachine().IsValid();

            Assert.IsFalse(ki.IsOnRightMachine().IsValid());

            ki.ActivatedMachines[0].Mid = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);

            Assert.IsTrue(ki.IsOnRightMachine().IsValid());
        }

        [TestMethod]
        public void HasExpiredTest()
        {
            var keyInfo = new LicenseKey()
            {
                Expires = DateTime.Today.AddDays(1)
            };


            Assert.IsTrue(keyInfo.HasNotExpired()
                                 .IsValid());


            keyInfo.Expires = DateTime.Today;

            Assert.IsTrue(keyInfo.HasNotExpired()
                                  .IsValid());

            Assert.IsTrue(keyInfo.HasNotExpired(checkWithInternetTime: true)
                                 .IsValid());


            keyInfo.Expires = DateTime.Today.AddDays(-1);

            Assert.IsTrue(!keyInfo.HasNotExpired()
                                  .IsValid());

        }

        [TestMethod]
        public void HasFeatureTest()
        {
            var ki = new LicenseKey()
            {
                Expires = DateTime.Today.AddDays(3),
                F1 = true,
                F2 = false,
                F5 = true
            };

            bool result = ki.HasFeature(1)
                            .HasNotFeature(2)
                            .HasFeature(5)
                            .HasNotExpired()
                            .IsValid();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasValidSigantureTest()
        {
            var ki = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyInfoResult>(TestCases.TestData.signedData).LicenseKey;

            Assert.IsTrue(ki.HasValidSignature(TestCases.TestData.pubkey).IsValid());
            Assert.IsFalse(ki.HasValidSignature(TestCases.TestData.pubkey, -1).IsValid());

            ki.Key = "d";

            Assert.IsFalse(ki.HasValidSignature(TestCases.TestData.pubkey).IsValid());

        }


    }
}
