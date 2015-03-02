using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace SKM_Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            //IWebProxy a = new IWebProxy();

            
        }

        [TestMethod]
        public void LengthEight()
        {
            int MUST_BE_LESS_THAN = 100000000;//1000000;

            //assuming not zero.
            for (int i = 0; i <= 1000000000; i++)
            {
                long hash = i;

                int result = (int)(hash % MUST_BE_LESS_THAN);

                if (result == 0)
                    result = 1;

                int check = MUST_BE_LESS_THAN / result;

                if (check > 1)
                {
                    result *= check;
                }

                if (MUST_BE_LESS_THAN == result)
                    result /= 10;


                if (result.ToString().Length != 8)
                    Assert.Fail();

            }

        }
    }
}
