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
    }
}
