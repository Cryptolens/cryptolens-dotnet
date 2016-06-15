using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKGL;
using SKM.V3;
using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM_Test
{

    [TestClass]
    public class TestWebAPI3
    {
      
        [TestMethod]
        public void AddDataObjectTest()
        {
            var keydata = new AddDataObjectModel() { };
            var auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

            var result = Data.AddDataObject(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                if (result.Id == 0)
                    Assert.Fail();

                var removeObj = Data.RemoveDataObject(auth, new RemoveDataObjectModel { Id = result.Id });

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
            var auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

            var result = Data.ListDataObjects(auth, keydata);
            
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
            var auth ="WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() {IntValue = 4711, Id = Id };
           
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
            var auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" ;

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
            var auth =  "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==" ;

            //first, let's obtain a random object. we record the old int value and the object id
            var objInt = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0];
            int oldInt = objInt.IntValue;
            long Id = objInt.Id;

            var keydata = new ChangeIntValueModel() { IntValue = 10, Id = Id };

            var result = Data.IncrementIntValue(auth, keydata);

            if (result != null && result.Result == ResultType.Success)
            {
                int objIntNew = Data.ListDataObjects(auth, new ListDataObjectsModel { ShowAll = true }).DataObjects[0].IntValue;
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
            var auth = "WyIxMSIsInRFLzRQSzJkT2V0Y1pyN3Y3a1I2Rm9YdmczNUw0SzJTRHJwUERhRFMiXQ==";

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



    

    }
}
