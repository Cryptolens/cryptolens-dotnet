using System;
using System.ComponentModel;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Methods;

namespace SKM_Test
{
    using SKGL;
    [TestClass]
    public class TestMachineCode
    {
        [TestMethod]
        public void UserNameMachineCodeTest()
        {
            string machineCode = SKM.getMachineCode(SKM.getSHA1);
            string machineCode2 =SKM.getMachineCode(SKM.getSHA1, true);

            Assert.AreNotEqual(machineCode, machineCode2);
        }

        [TestMethod]
        public void MachineCodeNew()
        {
            string mc = Helpers.GetMachineCodePI();
            string mc2 = Helpers.GetMachineCodePI(2);
            string mc3 = Helpers.GetMachineCodePI(10);

            Assert.IsTrue(mc == mc3);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var test = Helpers.GetHardiskId();
        }

        [TestMethod]
        public void GetWindowsMachineCodeV2UsesMachineGuidWhenUuidProcessStartIsDenied()
        {
            const string machineGuid = "test-machine-guid";

            var actual = Helpers.GetWindowsMachineCodeV2(
                () => { throw new Win32Exception(); },
                () => machineGuid);

            Assert.AreEqual(SKM.getSHA256(machineGuid, 2), actual);
        }

        [TestMethod]
        public void GetWindowsMachineCodeV2UsesMachineGuidWhenUuidAccessIsDenied()
        {
            const string machineGuid = "test-machine-guid";

            var actual = Helpers.GetWindowsMachineCodeV2(
                () => { throw new SecurityException(); },
                () => machineGuid);

            Assert.AreEqual(SKM.getSHA256(machineGuid, 2), actual);
        }

        [TestMethod]
        public void GetWindowsMachineCodeV2UsesMachineGuidWhenUuidIsEmpty()
        {
            const string machineGuid = "test-machine-guid";

            var actual = Helpers.GetWindowsMachineCodeV2(
                () => string.Empty,
                () => machineGuid);

            Assert.AreEqual(SKM.getSHA256(machineGuid, 2), actual);
        }

        [TestMethod]
        public void GetWindowsMachineCodeV2ReturnsNullWhenBothSourcesAreUnavailable()
        {
            var actual = Helpers.GetWindowsMachineCodeV2(
                () => { throw new Win32Exception(); },
                () => null);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetWindowsMachineCodeV2UsesUuidWithoutReadingMachineGuid()
        {
            const string uuid = "test-uuid";
            var machineGuidWasRead = false;

            var actual = Helpers.GetWindowsMachineCodeV2(
                () => uuid,
                () =>
                {
                    machineGuidWasRead = true;
                    return "test-machine-guid";
                });

            Assert.AreEqual(SKM.getSHA256(uuid, 2), actual);
            Assert.IsFalse(machineGuidWasRead);
        }
    }
}
