using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKGL;
using SKM.V3;
using SKM.V3.Models;
using SKM.V3.Internal;
using SKM.V3.Methods;

namespace SKM_Test
{

    [TestClass]
    public class TestWebAPI3
    {
        string auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";
        [TestMethod]
        public void AddDataObjectTest()
        {
            var keydata = new AddDataObjectModel() { };
            var result = Data.AddDataObject(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                if (result.Id == 0)
                    Assert.Fail();

                var removeObj = Data.RemoveDataObject(auth, new RemoveDataObjectModel { Id = result.Id });

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
        public void AddDataObjectToKeyTest()
        {
            var keydata = new AddDataObjectToKeyModel() { ProductId = 3941, Key = "FRQHQ-FSOSD-BWOPU-KJOWF" };
            var result = Data.AddDataObject(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                if (result.Id == 0)
                    Assert.Fail();

                var removeObj = Data.RemoveDataObject(auth, new RemoveDataObjectModel { Id = result.Id });

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
        public void ListDataObjectsTest()
        {
            var keydata = new ListDataObjectsModel { ShowAll = true };
            var result = Data.ListDataObjects(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                var firstObject = (DataObjectWithReferencer)result.DataObjects[0];

                if (firstObject.ReferencerId == 0)
                    Assert.Fail();
            }
            else
            {
                Assert.Fail();
            }

            keydata.ShowAll = false;

            result = Data.ListDataObjects(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                if (result.DataObjects[0] is DataObjectWithReferencer)
                    Assert.Fail();
            }
            else
            {
                Assert.Fail();
            }


        }

        [TestMethod]
        public void SetIntValueTest()
        {
            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 4711, Id = Id };

            var result = Data.SetIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
                Assert.IsTrue(objIntNew == 4711);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetStringValueTest()
        {
            //first, let's obtain a random object. we record the old string value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            string oldString = objInt.StringValue;
            long Id = objInt.Id;

            var keydata = new ChangeStringValueModel() { StringValue = "foo", Id = Id };

            var result = Data.SetStringValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                string objIntNew = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].StringValue;
                Assert.AreEqual(objIntNew, "foo");
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IncrementIntValueTest()
        {
            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 10, Id = Id };

            var result = Data.IncrementIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
                Assert.IsTrue(objIntNew == oldInt + 10);
            }
            else
            {
                Assert.Fail();
            }
        }


        [TestMethod]
        public void DecrementIntValueTest()
        {
            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 10, Id = Id };

            var result = Data.DecrementIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
                Assert.IsTrue(objIntNew == oldInt - 10);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddDataObjectAndListToKey()
        {
            var keydata = new AddDataObjectToKeyModel { ProductId = 3349, Key = "LEPWV-FOTPG-MWBEO-FBFPS", Name = "test123" };

            var result = Data.AddDataObject(auth, keydata);

            Assert.IsTrue(result != null && result.Result == ResultType.Success);

            int id = (int)result.Id; // the new id.

            var result2 = Data.ListDataObjects(auth, new ListDataObjectsToKeyModel { Key = "LEPWV-FOTPG-MWBEO-FBFPS", ProductId = 3349, Contains= "test123" });

            Assert.IsTrue(result2 != null && result2.Result == ResultType.Success);

            Assert.IsTrue(result2.DataObjects.Count > 0);

            bool found = false;
            foreach (var item in result2.DataObjects)
            {
                if (item.Name == "test123")
                    found = true;
            }

            Assert.IsTrue(found);

            var result3 = Data.RemoveDataObject(auth, new RemoveDataObjectToKeyModel { Key = "LEPWV-FOTPG-MWBEO-FBFPS", ProductId = 3349, Id = id });

            Assert.IsTrue(result3 != null && result3.Result == ResultType.Success);
        }


        [TestMethod]
        public void VerifyKeyAndCheckDataObjectTest()
        {
            //var licenseKey = "LZKZU-MPJEW-TARNP-UHDBQ";
            //var RSAPubKey = "<RSAKeyValue><Modulus>sGbvxwdlDbqFXOMlVUnAF5ew0t0WpPW7rFpI5jHQOFkht/326dvh7t74RYeMpjy357NljouhpTLA3a6idnn4j6c3jmPWBkjZndGsPL4Bqm+fwE48nKpGPjkj4q/yzT4tHXBTyvaBjA8bVoCTnu+LiC4XEaLZRThGzIn5KQXKCigg6tQRy0GXE13XYFVz/x1mjFbT9/7dS8p85n8BuwlY5JvuBIQkKhuCNFfrUxBWyu87CFnXWjIupCD2VO/GbxaCvzrRjLZjAngLCMtZbYBALksqGPgTUN7ZM24XbPWyLtKPaXF2i4XRR9u6eTj5BfnLbKAU5PIVfjIS+vNYYogteQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            //var auth = AccessToken.AccessToken.Activate;
            //var result = Key.Activate(token: auth, parameters: new ActivateModel()
            //{
            //    Key = licenseKey,
            //    ProductId = 3349,
            //    Sign = true,
            //    MachineCode = Helpers.GetMachineCode()
            //});

            //if (result == null || result.Result == ResultType.Error ||
            //    !result.LicenseKey.HasValidSignature(RSAPubKey).IsValid())
            //{
            //    // an error occurred or the key is invalid or it cannot be activated
            //    // (eg. the limit of activated devices was achieved)
            //    Console.WriteLine("The license does not work.");
            //}
            //else
            //{
            //    // everything went fine if we are here!
            //    Console.WriteLine("The license is valid!");

            //}

            //var obj = result.LicenseKey.DataObjects.Get("usagecount");

            //if (obj == null)
            //{
            //    // if it does not exist, add a new one.
            //    result.LicenseKey.AddDataObject(auth, new DataObject { Name = "usagecount", IntValue = 0 });
            //}

            //var res = obj.IncrementIntValue(auth, incrementValue: 1, licenseKey: result.LicenseKey);

            //Console.WriteLine(res ? "Success when updating the data object" : "Failure when updating the data object");

            //var result = Data.ListDataObjects(auth, new ListDataObjectsToKeyModel { Contains = "usagecount", Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349 });
            //var obj = result.DataObjects.Get("usagecount");

            //if (obj == null)
            //{
            //    Data.AddDataObject(auth, new AddDataObjectToKeyModel { Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349, Name = "usagecount", IntValue = 0 });
            //}
            //else
            //{
            //    var res = obj.IncrementIntValue("WyIxODY3IiwiT3RHbFlXcjBZNEVkT2JXUmVQekx2ZE1sZUNIVmdaL3VkQzRMc00zQiJd", incrementValue: 1, licenseKey: new LicenseKey { Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349 });
            //}


var result = Data.ListDataObjects("WyIxODY3IiwiT3RHbFlXcjBZNEVkT2JXUmVQekx2ZE1sZUNIVmdaL3VkQzRMc00zQiJd", new ListDataObjectsToKeyModel { Contains = "usagecoun", Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349 });
var obj = result.DataObjects.Get("usagecoun");

if (obj == null)
{
    var res = Data.AddDataObject("WyIxODY3IiwiT3RHbFlXcjBZNEVkT2JXUmVQekx2ZE1sZUNIVmdaL3VkQzRMc00zQiJd", new AddDataObjectToKeyModel { Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349, Name = "usagecoun", IntValue = 5 });

    if(res == null || res.Result == ResultType.Error)
    {
        Console.WriteLine("Could not create new data object." + res.Message);
    }
}
else
{
    var res = obj.DecrementIntValue("WyIxODY3IiwiT3RHbFlXcjBZNEVkT2JXUmVQekx2ZE1sZUNIVmdaL3VkQzRMc00zQiJd", decrementValue: 1, enableBound:true, lowerBound: 0, licenseKey: new LicenseKey { Key = "LZKZU-MPJEW-TARNP-UHDBQ", ProductId = 3349 });

    if (!res)
    {
        Console.WriteLine("Could not decrement the data object. The limit was reached.");
    }
}

        }
    }
}
