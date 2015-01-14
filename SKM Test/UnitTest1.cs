using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SKM_Test
{
    [TestClass]
    public class UnitTest1
    {   

        [TestMethod]
        public void TestMachineIDSignature()
        {

            // your public key can be found at http://serialkeymanager.com/Account/Manage.
            string rsaPublicKey = "<RSAKeyValue><Modulus>pL01ClSf7U5kp2E7C9qFecZGiaV8rFpET1u9QvuBrLNkCRB5mQFiaCqHyJd8Wj5o/vkBAenQO+K45hLQakve/iAmr4NX/Hca9WyN8DVhif6p9wD+FIGWeheOkbcrfiFgMzC+3g/w1n73fK0GCLF4j2kqnWrDBjaB4WfzmtA5hmrBFX3u9xcYed+dXWJW/I4MYmG0cQiBqR/P5xTTE+zZWOXwvmSZZaMvBh884H9foLgPWWsLllobQTHUqRq6pr48XrQ8GjV7oGigTImolenMLSR59anDCIhZy59PPsi2WE7OoYP8ecNvkdHWr1RlEFtx4bUZr3FPNWLm7QIq7AWwgw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            SKGL.KeyInformation keyInfo = new SKGL.KeyInformation();
            bool fileLoaded = false;

            try
            {
                keyInfo = SKGL.SKM.LoadKeyInformationFromFile("file007.txt");
                fileLoaded = true;
            }
            catch{}


            if(fileLoaded)
            {
                if (SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey))
                {
                    // if we've come so far, we know that
                    // * the key has been checked against the database once
                    // * the file with the key infromation has not been modified.

                    // check the key
                    if (keyInfo.Valid)
                    {
                        
                        // here we can retrive some useful info
                        Console.WriteLine(keyInfo.CreationDate);


                        //... etc.
                    }
                }
                else
                {
                    Assert.Fail();
                }

            }
            else
            {
                // it's crucial that both json and secure are set to true!
                //keyInfo = SKGL.SKM.KeyValidation("1012", "3", "941508", "LGNDO-LTHUB-MBRQV-PMLSJ", true,true);
                keyInfo = SKGL.SKM.KeyActivation("2202", "2", "882018", "LJEHZ-KRVNU-KTORH-KFKOV", "abc", true, true); // KeyActivation method works also.

                if(keyInfo.Valid)
                {
                    SKGL.SKM.SaveKeyInformationToFile(keyInfo, "file007.txt");
                }

            }
        }

        [TestMethod]
        public void KeyActivationTrial()
        {
            // client app

            string machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getEightByteHash);

            string keyToValidate = "KMAXI-BJHQS-HZXHT-XUTLS";

            var activationResult = SKGL.SKM.KeyActivation("3", "2", "751963", keyToValidate, machineCode);

            if (activationResult.Valid)
            {
                if (keyToValidate != activationResult.NewKey)
                {
                    // key has changed
                    // tell the user to store the new trial key somewhere
                    // maybe tell that activation was successful.
                }
                else
                {
                    // no update, so the key was already activated on this machine
                    // use SKGL to get more info.
                }
            }

            Assert.IsTrue(activationResult.Valid);
        }

        [TestMethod]
        public void KeyValidation()
        {
            // client app


            var validationResult = SKGL.SKM.KeyActivation("3", "2", "751963", "MNIVR-MGQRL-QGUZK-BGJHQ", "abc", true, false);

            var newKey = validationResult.NewKey;

            if (validationResult.Valid)
            {
                //valid key
                var created = validationResult.CreationDate;
                var expires = validationResult.ExpirationDate;
                var setTime = validationResult.SetTime;
                var timeLeft = validationResult.TimeLeft;
                var features = validationResult.Features;
                
            }
            else
            {
                //invalid key
                Assert.Fail();
            }


            //Assert.IsTrue(activationResult);
        }
        [TestMethod]
        public void SecureKeyValidation()
        {

            // your public key can be found at http://serialkeymanager.com/Account/Manage.
            string rsaPublicKey = "<RSAKeyValue><Modulus>pL01ClSf7U5kp2E7C9qFecZGiaV8rFpET1u9QvuBrLNkCRB5mQFiaCqHyJd8Wj5o/vkBAenQO+K45hLQakve/iAmr4NX/Hca9WyN8DVhif6p9wD+FIGWeheOkbcrfiFgMzC+3g/w1n73fK0GCLF4j2kqnWrDBjaB4WfzmtA5hmrBFX3u9xcYed+dXWJW/I4MYmG0cQiBqR/P5xTTE+zZWOXwvmSZZaMvBh884H9foLgPWWsLllobQTHUqRq6pr48XrQ8GjV7oGigTImolenMLSR59anDCIhZy59PPsi2WE7OoYP8ecNvkdHWr1RlEFtx4bUZr3FPNWLm7QIq7AWwgw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            SKGL.KeyInformation keyInfo = new SKGL.KeyInformation();
            bool fileLoaded = false;

            try
            {
                keyInfo = SKGL.SKM.LoadKeyInformationFromFile("file11112qavw.txt");
                fileLoaded = true;
            }
            catch{}


            if(fileLoaded)
            {
                if (SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey))
                {
                    // if we've come so far, we know that
                    // * the key has been checked against the database once
                    // * the file with the key infromation has not been modified.

                    // check the key
                    if (keyInfo.Valid)
                    {
                        
                        // here we can retrive some useful info
                        Console.WriteLine(keyInfo.CreationDate);


                        //... etc.
                    }
                }
                else
                {
                    Assert.Fail();
                }

            }
            else
            {
                // it's crucial that both json and secure are set to true!
                //keyInfo = SKGL.SKM.KeyValidation("1012", "3", "941508", "LGNDO-LTHUB-MBRQV-PMLSJ", true,true);
                keyInfo = SKGL.SKM.KeyValidation("2202", "2", "882018", "LJEHZ-KRVNU-KTORH-KFKOV", true); // KeyActivation method works also.

                if(keyInfo.Valid)
                {
                    SKGL.SKM.SaveKeyInformationToFile(keyInfo, "file11112qavw.txt");

                    Assert.IsTrue(SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey));
                }

            }




        }

        [TestMethod]
        public void SecureKeyActivation()
        {
            // your public key can be found at http://serialkeymanager.com/Account/Manage.
            string rsaPublicKey = "<RSAKeyValue><Modulus>pL01ClSf7U5kp2E7C9qFecZGiaV8rFpET1u9QvuBrLNkCRB5mQFiaCqHyJd8Wj5o/vkBAenQO+K45hLQakve/iAmr4NX/Hca9WyN8DVhif6p9wD+FIGWeheOkbcrfiFgMzC+3g/w1n73fK0GCLF4j2kqnWrDBjaB4WfzmtA5hmrBFX3u9xcYed+dXWJW/I4MYmG0cQiBqR/P5xTTE+zZWOXwvmSZZaMvBh884H9foLgPWWsLllobQTHUqRq6pr48XrQ8GjV7oGigTImolenMLSR59anDCIhZy59PPsi2WE7OoYP8ecNvkdHWr1RlEFtx4bUZr3FPNWLm7QIq7AWwgw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            SKGL.KeyInformation keyInfo = new SKGL.KeyInformation();
            bool fileLoaded = false;

            try
            {
                keyInfo = SKGL.SKM.LoadKeyInformationFromFile("file9913.txt");
                fileLoaded = true;
            }
            catch { }


            if (fileLoaded)
            {
                if (SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey))
                {
                    // if we've come so far, we know that
                    // * the key has been checked against the database once
                    // * the file with the key infromation has not been modified.

                    // check the key
                    if (keyInfo.Valid)
                    {

                        // here we can retrive some useful info
                        Console.WriteLine(keyInfo.CreationDate);


                        //... etc.
                    }
                }
                else
                {
                    Assert.Fail();
                }

            }
            else
            {
                // it's crucial that both json and secure are set to true!
                keyInfo = SKGL.SKM.KeyActivation("2202", "2", "882018", "LJEHZ-KRVNU-KTORH-KFKOV", "123", true, true); // KeyActivation method works also.

                if (keyInfo.Valid)
                {
                    SKGL.SKM.SaveKeyInformationToFile(keyInfo, "file9913.txt");
                }

            }

        }


        [TestMethod]
        public void GetParamtersTest()
        {
            var input = new System.Collections.Generic.Dictionary<string, string>();
            input.Add("uid", "2");
            input.Add("pid", "3");
            input.Add("hsum", "751963");
            input.Add("sid", "JLLKY-NEPGJ-BQCOR-EIJGY");
            input.Add("sign","true");

            
            var result = SKGL.SKM.GetParameters(input, "Validate");

            var keyinfo = SKGL.SKM.GetKeyInformationFromParameters(result);

            if(result.ContainsKey("error") && result["error"] != "")
            {
                Assert.Fail();
            }

            

            
        }
    }

}
