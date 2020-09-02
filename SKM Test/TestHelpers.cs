using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3;
using SKM.V3.Methods;
using SKM.V3.Models;

namespace SKM_Test
{
    [TestClass]
    public class UnitTest2
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
    }
}
