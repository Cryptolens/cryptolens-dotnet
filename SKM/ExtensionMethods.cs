using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKGL
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyInformation"></param>
        /// <returns></returns>
        public static bool IsValid(this KeyInformation keyInformation)
        {
            if(keyInformation != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyInformation"></param>
        /// <param name="rsaPublicKey"></param>
        /// <returns></returns>
        public static bool IsValid(this KeyInformation keyInformation, string rsaPublicKey)
        {
            if (keyInformation != null)
            {
                if(!SKGL.SKM.IsKeyInformationGenuine(keyInformation, rsaPublicKey))
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        

        /// <summary>
        /// Checks that they key has not expired (i.e. the expire date has not been reached).
        /// </summary>
        /// <param name="keyInformation"></param>
        /// <param name="checkWithInternetTime">If set to true, we will also check that the local
        /// time (on the client computer) has not been changed (using SKM.TimeCheck). 
        /// </param>
        /// <returns>A key information object if the condition is satisfied. Null otherwise.</returns>
        public static KeyInformation HasNotExpired(this KeyInformation keyInformation, bool checkWithInternetTime = false)
        {
            if (keyInformation != null)
            {
                TimeSpan ts = keyInformation.ExpirationDate - DateTime.Today;

                if (ts.Days >= 0)
                {
                    if (checkWithInternetTime && !SKGL.SKM.TimeCheck())
                        return null;
                    return keyInformation;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyInformation"></param>
        /// <param name="rsaPublicKey"></param>
        /// <returns>A key information object if the condition is satisfied. Null otherwise.</returns>
        public static KeyInformation HasValidSignature(this KeyInformation keyInformation, string rsaPublicKey)
        {
            if (keyInformation != null)
            {
                if (SKGL.SKM.IsKeyInformationGenuine(keyInformation, rsaPublicKey))
                {
                    return keyInformation;
                }
            }
            return null;
        }
        
    }
}
