using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SKM_Test
{
    using SKGL;
    [TestClass]
    public class TestSaveAndLoadFile
    {
        [TestMethod]
        public void SaveToFileBinaryFormatterTest()
        {
            var ki = new KeyInformation() { CreationDate=DateTime.Today, NewKey="test", Mid = "123" };

            // using binary formatter
            SKM.SaveKeyInformationToFile(ki, "test.txt");

            var ki2 = new KeyInformation();

            //loading as usual
            ki2 = SKM.LoadKeyInformationFromFile("test.txt");

            Assert.AreEqual(ki.CreationDate, ki2.CreationDate);
            Assert.AreEqual(ki.NewKey, ki2.NewKey);
            Assert.AreEqual(ki.Mid, ki2.Mid);

        }


        [TestMethod]
        public void SaveToFileJSONNormalFormatTest()
        {
            var ki = new KeyInformation() { CreationDate = DateTime.Today, NewKey = "test", Mid = "123" };

            // using binary formatter
            SKM.SaveKeyInformationToFile(ki, "test.txt", json:true);

            var ki2 = new KeyInformation();

            //loading as usual
            ki2 = SKM.LoadKeyInformationFromFile("test.txt", json: true);

            Assert.AreEqual(ki.CreationDate, ki2.CreationDate);
            Assert.AreEqual(ki.NewKey, ki2.NewKey);
            Assert.AreEqual(ki.Mid, ki2.Mid);

        }
    }
}
