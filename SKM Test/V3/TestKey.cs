using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Models;
using System.Collections.Generic;
using SKM.V3.Internal;
using SKM.V3.Methods;

namespace SKM_Test
{
    [TestClass]
    public class TestKey
    {
        [TestMethod]
        public void SaveLoadFile()
        {
            var key = new LicenseKey() { Key = "hello", Expires = DateTime.Today };

            key.SaveToFile();

            var load = new LicenseKey().LoadFromFile();

            Assert.IsTrue(key.Key == load.Key);

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
        [TestMethod]
        public void ExtendLicenseTest()
        {
            var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
            var auth = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd";

            var result = Key.ExtendLicense(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {

                // the license was successfully extended with 30 days.
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddFeatureTest()
        {
            var keydata = new FeatureModel() { Key = "LXWVI-HSJDU-CADTC-BAJGW", Feature = 2, ProductId = 3349 };
            var auth = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";

            var result = Key.AddFeature(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                // feature 2 is set to true.
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveFeatureTest()
        {
            var keydata = new FeatureModel() { Key = "LXWVI-HSJDU-CADTC-BAJGW", Feature = 2, ProductId = 3349 };
            var auth = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";

            var result = Key.RemoveFeature(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                // feature 2 is set to true.
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ActivateTest()
        {
            var auth = "WyIxMTgiLCJkN0dEREZ0YW03alNhRVNtV3dOQkxZdjJlMWFTVlpacjNVaisxNFBZIl0=";
            var result = Key.Activate(token: auth, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" });
            System.Diagnostics.Debug.WriteLine(result);


            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            Assert.IsTrue(result.LicenseKey.IsValid(TestCases.TestData.pubkey));

            result.LicenseKey.Signature = "test";

            Assert.IsFalse(result.LicenseKey.IsValid(TestCases.TestData.pubkey));
        }

        [TestMethod]
        public void KeyLockTest()
        {
            // make sure the access token below has key lock set to "-1".
            // the token below has key lock set to "-1" and access to "AddFeature" method.
            // note, we cannot use this token for anything but the Key Lock method, in order
            // to get a new token.
            var auth = "WyI0NCIsInRhOGNJZm1BS0xkbGJjUW55UkdEN3lzTzhWckd6SzRzYlgvRkFOQmQiXQ==";

            // 1. Get a new token
            var key = "ITVBC-GXXNU-GSMTK-NIJBT";
            var result = Auth.KeyLock(auth, new KeyLockModel { Key = key, ProductId = 3349 });

            var newAuth = result.Token;

            // 2. Access the method
            var addFeature = Key.AddFeature(newAuth, new FeatureModel { Feature = 2, ProductId = 3349, Key = key });

            // this should work
            if (addFeature.Result == ResultType.Error)
                Assert.Fail();

            var wrongKey = "MTMPW-VZERP-JZVNZ-SCPZM";
            var addFeatureWrongKey = Key.AddFeature(newAuth, new FeatureModel { Feature = 2, ProductId = 3, Key = wrongKey });

            // this should not work
            if (addFeatureWrongKey != null && addFeatureWrongKey.Result == ResultType.Success)
                Assert.Fail();

        }

        [TestMethod]
        public void SignatureVerificationTest()
        {
            var keyInfoResult = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyInfoResult>(TestCases.TestData.signedData);

            var license = keyInfoResult.LicenseKey;

            Assert.IsTrue(license.IsValid(TestCases.TestData.pubkey));

        }

        [TestMethod]
        public void CreateKeyTest()
        {
            var newKey = Key.CreateKey("access token", 
                                       new CreateKeyModel {ProductId = 3349 });
            System.Diagnostics.Debug.WriteLine(newKey?.Key);
        }

        [TestMethod]
        public void AddDataObjectKeyTest()
        {
            var keydata = new AddDataObjectModel() { };
            var auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

            var auth2 = "WyIxMTgiLCJkN0dEREZ0YW03alNhRVNtV3dOQkxZdjJlMWFTVlpacjNVaisxNFBZIl0=";
            var license = Key.Activate(token: auth2, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" });

            long id = license.LicenseKey.AddDataObject(auth, new DataObject { });

            if (id > 0)
            {
                var removeObj = Data.RemoveDataObject(auth, new RemoveDataObjectModel { Id = id });

                if (removeObj == null || removeObj.Result == ResultType.Error)
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RefreshTest()
        {
            var activateToken = "WyIxMTgiLCJkN0dEREZ0YW03alNhRVNtV3dOQkxZdjJlMWFTVlpacjNVaisxNFBZIl0=";
            var featureToken = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";

            var license = Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" }).LicenseKey;

            bool feature1 = license.HasFeature(1).IsValid();

            license.Refresh(activateToken);

            // TODO:
        }

        [TestMethod]
        public void TestKeyBlock()
        {
            string token = "WyIxNzIiLCJhak9OT1g3NW90YlQyRFFVUzBWdnlGSHJYdUpMdDA0REMxNzNOa2duIl0=";
            string key = "LEPWV-FOTPG-MWBEO-FBFPS";

            // this is a simple hack to retrieve license info (auto complete it).
            var license = new LicenseKey { ProductId = 3349, Key = key };
            license.Refresh(token);

            Key.BlockKey(token, new KeyLockModel { Key = key, ProductId = 3349 });

            license.Refresh(token);

            Assert.IsTrue(license.IsBlocked().IsValid());

            Key.UnblockKey(token, new KeyLockModel { Key = key, ProductId = 3349 });

            license.Refresh(token);

            Assert.IsTrue(license.IsNotBlocked().IsValid());


        }

    }
}
