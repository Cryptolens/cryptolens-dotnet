﻿using System;
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
        string activateDeactivate = "WyIxNzkiLCJQa3l3Rm00NFJrUVdVVHFnai84TUxtbStYTm5VUzdJNVpLcGNueTVXIl0=";
        string getKeys = "WyIxNzIiLCJhak9OT1g3NW90YlQyRFFVUzBWdnlGSHJYdUpMdDA0REMxNzNOa2duIl0=";


        [TestMethod]
        public void MyTestMethod()
        {
            var username = "testuser";
            var password = Helpers.ComputePasswordHash("testpassword");

            // Adding a user
            var res = Key.Activate(AccessToken.AccessToken.Activate, new ActivateModel 
            { 
                Key = "KMZEW-SBRAE-VWCEK-CDLQE", 
                ProductId = 3349,
                MachineCode = password,
                FriendlyName = username
            });

            Assert.IsTrue(Helpers.IsSuccessful(res));


            //verifying password
            var res2 = Key.GetKey(AccessToken.AccessToken.GetKey, new KeyInfoModel
            {
                Key = "KMZEW-SBRAE-VWCEK-CDLQE",
                ProductId = 3349
            });

            Assert.IsTrue(Helpers.VerifyPassword(res2.LicenseKey, "testuser", "testpassword"));
            Assert.IsFalse(Helpers.VerifyPassword(res2.LicenseKey, "testuser", "testpassword2"));

        }


        [TestMethod]
        public void MachineLockTest()
        {
            var key = "BIJGF-ZNULN-FVALJ-GQDTH";
            var license = Key.GetKey(AccessToken.AccessToken.GetKey, new KeyInfoModel { Key = key, ProductId = 3349 });

            if (license == null || license.Result == ResultType.Error)
                Assert.Fail("Could not get the key: " + license.Message );

            Random rnd = new Random();
            var num = rnd.Next(1, 999);

            var increaseMachineLockLimit = Key.MachineLockLimit(AccessToken.AccessToken.MachineLoclLimit, new MachineLockLimit { Key = key, ProductId = 3349, NumberOfMachines = num });

            if (increaseMachineLockLimit == null || increaseMachineLockLimit.Result == ResultType.Error)
                Assert.Fail("Could not update.");

            license = Key.GetKey(AccessToken.AccessToken.GetKey, new KeyInfoModel { Key = key, ProductId = 3349 });

            if (license == null || license.Result == ResultType.Error)
                Assert.Fail("Could not get the key 2nd time.");

            Assert.IsTrue(license.LicenseKey.MaxNoOfMachines == num);

        }


        [TestMethod]
        public void SignatureTest()
        {
            var result = Key.Activate(AccessToken.AccessToken.Activate, new ActivateModel {  MachineCode = "test", Key = "MTMPW-VZERP-JZVNZ-SCPZM", ProductId = 3349, Sign = true});

            if(result == null || result.Result == ResultType.Error)
            {
                Assert.Fail();
            }

            if (!result.LicenseKey.HasValidSignature("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", 30).IsValid())
                Assert.Fail();
        }

        [TestMethod]
        public void TestGetKey2()
        {
            var result = Key.GetKey(AccessToken.AccessToken.GetKey, key: "MTMPW-VZERP-JZVNZ-SCPZM", productId: 3349);

            var rsa = "<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var res = LicenseKey.FromResponse(rsa, result);

            Assert.IsTrue(res != null);

        }

        [TestMethod]
        public void SignatureTestNewMethod()
        {
            var result = Key.Activate(AccessToken.AccessToken.Activate, 3349, "MTMPW-VZERP-JZVNZ-SCPZM", "test");

            if (result == null || result.Result == ResultType.Error)
            {
                Assert.Fail();
            }

            var license = LicenseKey.FromResponse("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", result);

            Assert.IsNotNull(license);
      
        }

        //[TestMethod]
        //public void SaveLoadFile()
        //{
        //    var key = new LicenseKey() { Key = "hello", Expires = DateTime.Today };

        //    if (key.SaveToFile() == null)
        //        Assert.Fail();

        //    var load = new LicenseKey().LoadFromFile();

        //    Assert.IsTrue(key.Key == load.Key);

        //}

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
        public void ExtendLicenseChangeExpireDateTest()
        {
            var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 365, ProductId = 3349 };
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
        public void GetKeyTest()
        {
            var res = Key.GetKey(AccessToken.AccessToken.GetKey, new KeyInfoModel { Metadata = true, ProductId = 3349, Key = "MTMPW-VZERP-JZVNZ-SCPZM" });

            if(res.Result == ResultType.Error)
            {
                Assert.Fail();
            }

            if(res.Metadata == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ActivateAndDeactivateTest()
        {
            var auth = activateDeactivate;
            var result = Key.Activate(token: auth, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo", FriendlyName = "TEST-PC"  });
            System.Diagnostics.Debug.WriteLine(result);

            //Assert.IsTrue(result.LicenseKey.HasValidSignature("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>").IsValid());
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            Assert.IsTrue(result.LicenseKey.IsValid(TestCases.TestData.pubkey));

            result.LicenseKey.Signature = "test";

            Assert.IsFalse(result.LicenseKey.IsValid(TestCases.TestData.pubkey));


            var result2 = Key.Deactivate(auth, new DeactivateModel { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, MachineCode = "foo" });

            if(result2 == null || result2.Result == ResultType.Error)
            {
                Assert.Fail("The deactivation did not work");
            }

        }

        [TestMethod]
        public void ActivateAndDeactivateNewProtocolTest()
        {
            var auth = activateDeactivate;
            var result = Key.Activate(token: auth, productId: 3349, key: "GEBNC-WZZJD-VJIHG-GCMVD", machineCode: "foo");

            System.Diagnostics.Debug.WriteLine(result);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result == ResultType.Success);

            Assert.IsTrue(LicenseKey.FromResponse(TestCases.TestData.pubkey, result) != null);

            var license = LicenseKey.FromResponse(TestCases.TestData.pubkey, result);

            license.SaveToFile("newprotocol.skm");

            Assert.IsTrue(license.HasValidSignature(TestCases.TestData.pubkey).IsValid());

            license = new LicenseKey().LoadFromFile("newprotocol.skm", TestCases.TestData.pubkey);

            Assert.IsTrue(license != null);
            license = new LicenseKey().LoadFromFile("newprotocol.skm");

            Assert.IsFalse(license != null);


            var result3 = Key.Activate(auth, new ActivateModel() { ProductId =  3349,  Key= "GEBNC-WZZJD-VJIHG-GCMVD", MachineCode= "foo", Sign=true});

            result3.LicenseKey.SaveToFile("oldprotocol.skm");

            Assert.IsTrue(new LicenseKey().LoadFromFile("oldprotocol.skm") != null);
            Assert.IsTrue(new LicenseKey().LoadFromFile("oldprotocol.skm", TestCases.TestData.pubkey) != null);


            //result.LicenseKey.Signature = "test";

            //Assert.IsFalse(result.LicenseKey.IsValid(TestCases.TestData.pubkey));


            var result2 = Key.Deactivate(auth, new DeactivateModel { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, MachineCode = "foo" });

            if (result2 == null || result2.Result == ResultType.Error)
            {
                Assert.Fail("The deactivation did not work");
            }

        }

        [TestMethod]
        public void UsageCounter()
        {
            var token = getKeys;
            var tokenDObj = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

            // this is a simple hack to retrieve license info (auto complete it).
            var license = new LicenseKey { ProductId = 3349, Key = "GEBNC-WZZJD-VJIHG-GCMVD" };
            license.Refresh(token);
            
            if(license.DataObjects.Contains("usagecount"))
            {
                var dataObj = license.DataObjects.Get("usagecount");

                // attempt to increment. true means we succeed.
                if (dataObj.IncrementIntValue(token: tokenDObj,
                                             incrementValue: 1,
                                             enableBound: true,
                                             upperBound: 4,
                                             licenseKey: license))
                {
                    // success, we can keep using this feature
                }
                else
                {
                    dataObj.SetIntValue(tokenDObj, 0);
                    Assert.Fail();
                    // fail, the the user has already used it 10 times.
                }
              
            }
            else
            {
                // if it does not exist, add a new one.
                license.AddDataObject(tokenDObj, new DataObject { Name = "usagecount", IntValue = 0 });
            }


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
        public void AddDataObjectKeyTest()
        {
            var keydata = new AddDataObjectModel() { };
            var auth = AccessToken.AccessToken.DataObjectaAll;

            var auth2 = activateDeactivate;
            var license = Key.Activate(token: auth2, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" });

            long id = license.LicenseKey.AddDataObject(auth, new DataObject {  });

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
            var activateToken = activateDeactivate;
            var featureToken = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";

            var license = Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" }).LicenseKey;

            bool feature1 = license.HasFeature(1).IsValid();

            license.Refresh(activateToken);

            // TODO:
        }

        [TestMethod]
        public void KeyBlockTest()
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

        [TestMethod]
        public void KeyDateTest()
        {
            var license = new LicenseKey()
            {
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(300)
            };

            Assert.IsTrue(license.DaysLeft() == 300);
        }

        [TestMethod]
        public void ChangeNotesTest()
        {
            var result = Key.ChangeNotes(AccessToken.AccessToken.ChangeNotes, new ChangeNotesModel { Key = "LEPWV-FOTPG-MWBEO-FBFPS", Notes = "test", ProductId = 3349 });

            if(result == null || result.Result == ResultType.Error)
            {
                Assert.Fail("Error on the server side.");
            }
        }


        [TestMethod]
        public void LicenseStatusTest()
        {
            var test = new LicenseKey();

            var res = Newtonsoft.Json.JsonConvert.SerializeObject(test);

            var activateToken = activateDeactivate;
            var featureToken = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";

            var result = Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "foo" , Metadata=true});

            var license = result.LicenseKey;

            Assert.IsTrue(license.IsValid());
            Assert.IsTrue(result.Metadata.LicenseStatus.IsValid);

            Assert.IsTrue(result.Metadata.VerifySignature("<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"));
        }

        [TestMethod]
        public void FloatingLicensingTest()
        {
            var activateToken = activateDeactivate;
            var res1 = Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = "test", Metadata = true, FloatingTimeInterval = 10 });

            Key.Deactivate(activateToken, new DeactivateModel { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, MachineCode = "test", Floating = true });

            var res2= Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = Helpers.GetMachineCode(), Metadata = true, FloatingTimeInterval = 10, MaxOverdraft=1 });

            Assert.IsTrue(Helpers.IsOnRightMachine(res2.LicenseKey, isFloatingLicense: true, allowOverdraft: true));

            var activateModel = new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = Helpers.GetMachineCode(), Metadata = true, FloatingTimeInterval = 10, MaxOverdraft = 1 };
            var activateResult = Key.Activate(token: activateToken, parameters: activateModel);

            if (activateResult != null && activateResult.Result == ResultType.Success)
            {
                var info = Helpers.GetFloatingLicenseInformation(activateModel, activateResult);

                System.Diagnostics.Debug.WriteLine(info.AvailableDevices);
                System.Diagnostics.Debug.WriteLine(info.UsedDevices);
                System.Diagnostics.Debug.WriteLine(info.OverdraftDevices);

                var a = info.AvailableDevices;
            }
            else
            {
                Assert.Fail();
            }

            //var result = Key.Activate(token: activateToken, parameters: new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, Sign = true, MachineCode = Helpers.GetMachineCode(), Metadata = true, FloatingTimeInterval = 100 });

            //Assert.IsTrue(result.LicenseKey.ActivatedMachines[0].Mid.StartsWith("floating:"));

            //Assert.IsTrue(Helpers.IsOnRightMachine(result.LicenseKey, isFloatingLicense: true));
        }

        [TestMethod]
        public void CreateTrialKeyTest()
        {
            var newTrialKey = Key.CreateTrialKey(AccessToken.AccessToken.CreateTrialKey, new CreateTrialKeyModel { ProductId= 3941, MachineCode = Helpers.GetMachineCode() });

            if(newTrialKey == null || newTrialKey.Result == ResultType.Error)
            {
                Assert.Fail("Something went wrong when creating the trial key");
            }

            var activate = Key.Activate(AccessToken.AccessToken.ActivateAllProducts, 
                new ActivateModel {
                    ProductId = 3941,
                    Sign = true,
                    MachineCode = Helpers.GetMachineCode(),
                    Key = newTrialKey.Key, Metadata = true
                });

            if(activate == null || activate.Result == ResultType.Error)
            {
                Assert.Fail("Something went wrong when verifying the trial key");
            }

            // now we can verify some basic properties

            if (Helpers.IsOnRightMachine(activate.LicenseKey))
            {
                // license verification successful.

                return;
            }

            Assert.Fail();

        }

        [TestMethod]
        public void CreateKeyTest()
        {
            var res = Key.CreateKey(AccessToken.AccessToken.CreateKeyTestProd, new CreateKeyModel { ProductId = 3941, F1 = true, Notes = "Test Create Key" });

            if(res == null || res.Result == ResultType.Error)
            {
                Assert.Fail();
            }
        }
    }
}
