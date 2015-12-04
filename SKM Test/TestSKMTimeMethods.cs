using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKGL;
using System.Diagnostics;
namespace SKM_Test
{
    [TestClass]
    public class TestSKMTimeMethods
    {
        [TestMethod]
        public void HasLocalTimeChangedTest()
        {
            bool hasChanged = SKGL.SKM.TimeCheck();

            if (hasChanged)
            {
                Assert.Fail();
                Debug.WriteLine("The local time was changed by the user. Validation fails.");
            }
            else
            {
                Debug.WriteLine("The local time hasn't been changed. Continue validation.");
            }

        }

        [TestMethod]
        public void DaysLeft()
        {
            var ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(30)};
            Assert.IsTrue(SKM.DaysLeft(ki) == 30);

            ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today };
            Assert.IsTrue(SKM.DaysLeft(ki) == 0);

            ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(-3) };
            Assert.IsTrue(SKM.DaysLeft(ki) == -3);

            ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(-3) };
            Assert.IsTrue(SKM.DaysLeft(ki, true) == 0);

            ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today };
            Assert.IsTrue(SKM.DaysLeft(ki, true) == 0);

            ki = new KeyInformation() { CreationDate = DateTime.Today, ExpirationDate = DateTime.Today.AddDays(-1) };
            Assert.IsTrue(SKM.DaysLeft(ki, true) == 0);
        }
    }
}
