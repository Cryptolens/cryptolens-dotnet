using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKGL;

namespace SKM_Test
{
    [TestClass]
    public class TestMachineCode
    {
        [TestMethod]
        public void UserNameMachineCodeTest()
        {
            string machineCode = SKGL.SKM.getMachineCode(SKM.getSHA1);
            string machineCode2 = SKGL.SKM.getMachineCode(SKM.getSHA1, true);

            Assert.AreNotEqual(machineCode, machineCode2);
        }
    }
}
