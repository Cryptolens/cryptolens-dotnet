using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class MethodSpecificTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a = Helpers.GetMachineCodePI();
            System.Diagnostics.Debug.WriteLine(Helpers.GetMachineCodePI());
            System.Diagnostics.Debug.WriteLine(Helpers.GetMachineCodePI());

            System.Diagnostics.Debug.WriteLine(Helpers.GetMachineCodePI());

        }

        [TestMethod]
        public void MyTestMethod()
        {
            System.Diagnostics.Debug.WriteLine(Environment.OSVersion.Platform.ToString());

            AI.RegisterEvent("", new RegisterEventModel { Metadata = Helpers.GetOSStats() });

        }


        [TestMethod]
        public void CheckVM()
        {
            Helpers.IsVM();
        }

        [TestMethod]
        public void CheckGetMachineCodeNormal()
        {
            Helpers.GetMachineCode(platformIndependent: false);
        }

        [TestMethod]
        public void TestHasFeature()
        {
            var features = "[\"f1\", [\"f2\",[[\"voice\",[\"all\"]], \"image\"]]]";

            var lc = new LicenseKey();
            lc.DataObjects = new System.Collections.Generic.List<DataObject>();
            lc.DataObjects.Add(new DataObject() { Name = "cryptolens_features", StringValue = features });
            
            
            Assert.IsTrue(Helpers.HasFeature(lc, "f2.voice.all"));
            Assert.IsTrue(Helpers.HasFeature(lc, "f2.voice"));
            Assert.IsTrue(Helpers.HasFeature(lc, "f2"));
            Assert.IsFalse(Helpers.HasFeature(lc, "f1.voice"));
        }
    }
}
