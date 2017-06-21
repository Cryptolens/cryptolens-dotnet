using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Methods;
using SKM.V3.Models;
using SKM.V3;

namespace SKM_Test
{
    [TestClass]
    public class TestHelpers
    {
        [TestMethod]
        public void MachineCodeTest()
        {
            var m = Helpers.GetMachineCode();

            var license = new LicenseKey();

            Assert.IsFalse(Helpers.IsOnRightMachine(license));

            license.ActivatedMachines = new System.Collections.Generic.List<ActivationData>();

            license.ActivatedMachines.Add(new ActivationData() { Mid = m});

            Assert.IsTrue(Helpers.IsOnRightMachine(license));
        }
    }
}
