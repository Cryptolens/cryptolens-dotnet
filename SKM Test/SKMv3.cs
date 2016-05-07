using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKM.V3;
using SKGL;

namespace SKM_Test
{
    [TestClass]
    public class SKMv3
    {
        [TestMethod]
        public void TestActivate()
        {
            var auth = new AuthDetails {Token  = "WyIxMTgiLCJkN0dEREZ0YW03alNhRVNtV3dOQkxZdjJlMWFTVlpacjNVaisxNFBZIl0=" } ;
            var result = Key.Activate(new ActivateModel() { Key = "GEBNC-WZZJD-VJIHG-GCMVD", ProductId = 3349, MachineCode="foo" }, auth);

        }
    }
}
