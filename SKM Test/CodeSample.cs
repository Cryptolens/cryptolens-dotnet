using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SKM_Test
{
    [TestClass]
    public class CodeSample
    {
        [TestMethod]
        public void KeyValidation()
        {
            /*
             * STEP 1: MACHINE CODE
             * 
             * Somewhere in the software, you have to tell the user their machine code.
             * You can also use SHA1 for machine code generation.
             * 
             * */

            string machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getEightDigitsLongHash);


            /**
             * 
             * STEP 2: VALIDATION OF ACTIVATION FILE
             * 
             * The code is already pre-configured for you; you only need to specify the file path.
             * You can specify the file path in the string filePath variable below
             * 
             * */

            string filePath = "c:\\out\\test.skm";


            string rsaPublicKey = "<RSAKeyValue><Modulus>tkNyLr8jUXU/eNvFt26Fd5japs9SxuVmTYG71SLLuyFImyhX8AseWj7o5yWJE04wr8MHBNjkx3EaoKU9AeXx876i3o2NQG9n2KJ4CoAslw3U+1YOD9NQunPNJdyKqSxqaMgRuahi184pnOjLyXIJdwrZvwa6+uvANhYxcNR9UMG3wfUpIDMqyAVt2GmHvR+m+/IDPeuUZFvAvbzE85bSpVSffh94v1IjD6qWXRM415iiYW9c6vXuHjvLBGN9CzQes0zhwz2MDZzVdVrvbbvchg67dJvnXP0glNMaUMhQLkq0xRrvdwOfV9AqCHxCzTVo8k2ZL5WTNUnMm1N6fvDIEw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            SKGL.KeyInformation keyInfo = new SKGL.KeyInformation();
            bool fileLoaded = false;

            
            try
            {
                keyInfo = SKGL.SKM.LoadKeyInformationFromFile(filePath, json: true);
                fileLoaded = true;
            }
            catch{ }

            if (fileLoaded)
            {
                if (SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey))
                {
                    // if we have come so far, the user has a valid activation file
                    // where no information has been altered. 
                    // below this line, please put some logic handle 


                    if(!machineCode.Equals(keyInfo.Mid))
                    {
                        // if this is not true, the user tries to activate
                        // an activation file that belongs to another computer.
                        // therefore, it's invalid operation and should FAIL.
                    }

                    // here we can retrive some useful info
                    Console.WriteLine(keyInfo.CreationDate);
                    //... etc.
                }
            }
        }
    }
      

    


}
