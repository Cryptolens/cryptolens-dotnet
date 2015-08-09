using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKGL;
namespace SKM_Test
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void IsValidTest()
        {
            var keyInfo = new KeyInformation();

            bool v = keyInfo.HasNotExpired()
                            .IsValid();
        }
    }
}
