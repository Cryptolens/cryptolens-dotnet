using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SKM_Test
{
    [TestClass]
    public class Support
    {

        /*
         * The code below is already pre-configured for the specified product.
         * You only need to change "{sign the data}" to either true or false - (if true, the server will sign the result)
         * and {sign machine code} to either true or false - (if true, the server will include the machine code in the signature).
         * {sign the data} corresponds to "sign" and {sign machine code} corresponds to "signMid" in the Web API.
         *
         * For more information: http://docs.serialkeymanager.com/web-api/activation/
         */
        [TestMethod]
        public void TimeLimitedKeysFeatureLocking()
        {
            var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);

            var activationResult = SKGL.SKM.KeyActivation("2196", "2", "749172", "serial key to validate", machineCode);
 
            if (activationResult != null)
            {
                // the key is valid (it might have expired, but at least it's not blocked).

                if(activationResult.Features[0]) // this basically says that the key is time limited.
                {
                    if(activationResult.TimeLeft >= 0) // the time check.
                    {
                        // everything is ok.
                    }
                    else
                    {
                        // tell the user that the key has expired.
                    }
                }
                else
                {
                    // If this feature was false, maybe this is a different key, so we could have different logic heere. 
                    // otherwise, you could also treat this as invalid.
                }
            }
            else
            {
                //invalid key
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TimeLimitedKeysFeatureLockingOffline()
        {
            var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);

            var activationResult = SKGL.SKM.KeyActivation("2196", "2", "749172", "LPGQX-KBKUY-JZNDO-TLPJO", machineCode);

            if (activationResult != null)
            {
                // the key is valid (it might have expired, but at least it's not blocked).

                if (activationResult.Features[0]) // this basically says that the key is time limited.
                {
                    TimeSpan diff = DateTime.Now - activationResult.CreationDate;
                    if (activationResult.SetTime  >= diff.Days) // the time check.
                    {
                        // everything is ok.
                    }
                    else
                    {
                        // tell the user that the key has expired.
                    }
                }
                else
                {
                    // If this feature was false, maybe this is a different key, so we could have different logic heere. 
                    // otherwise, you could also treat this as invalid.
                }
            }
            else
            {
                //invalid key
                Assert.Fail();
            }
        }
    }
}
