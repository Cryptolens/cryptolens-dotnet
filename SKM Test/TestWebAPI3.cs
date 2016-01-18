﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKGL;

namespace SKM_Test
{
    [TestClass]
    public class TestWebAPI3
    {
        [TestMethod]
        public void ExtendLicenseTest()
        {
            var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
            var auth = new AuthDetails() { Token = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd" };

            var result = SKM.ExtendLicense(auth, keydata);

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
            var auth = new AuthDetails() { Token = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd" };

            var result = SKM.AddFeature(auth, keydata);

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
            var auth = new AuthDetails() { Token = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd" };

            var result = SKM.RemoveFeature(auth, keydata);

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
        public void AddDataObjectTest()
        {
            var keydata = new AddDataObjectModel() { };
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            var result = SKM.AddDataObject(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                if (result.Id == 0)
                    Assert.Fail();

                var removeObj = SKM.RemoveDataObject(auth, new RemoveDataObjectModel { Id = result.Id });

                if(removeObj == null || removeObj.Result == ResultType.Error)
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
            var keydata = new ListDataObjectsModel {  ShowAll = true };
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            var result = SKM.ListDataObjects(auth, keydata);
            
            if (result != null && result.Result == ResultType.Success)
            {
                var firstObject =  (DataObjectWithReferencer)result.DataObjects[0];

                if (firstObject.ReferencerId == 0)
                    Assert.Fail();
            }
            else
            {
                Assert.Fail();
            }

            keydata.ShowAll = false;

            result = SKM.ListDataObjects(auth, keydata);

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
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() {IntValue = 4711, Id = Id };
           
            var result = SKM.SetIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
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
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            //first, let's obtain a random object. we record the old string value and the object id
            var objInt = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            string oldString = objInt.StringValue;
            long Id = objInt.Id;

            var keydata = new ChangeStringValueModel() { StringValue = "foo", Id = Id };

            var result = SKM.SetStringValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                string objIntNew = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].StringValue;
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
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 10, Id = Id };

            var result = SKM.IncrementIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
                Assert.IsTrue(objIntNew == oldInt+10);
            }
            else
            {
                Assert.Fail();
            }
        }


        [TestMethod]
        public void DecrementIntValueTest()
        {
            var auth = new AuthDetails() { Token = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" };

            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 10, Id = Id };

            var result = SKM.DecrementIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = SKM.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
                Assert.IsTrue(objIntNew == oldInt - 10);
            }
            else
            {
                Assert.Fail();
            }
        }

    }
}
