using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKGL
{
    public static class ExtensionMethods
    {
        public static bool IsValid(this KeyInformation keyInformation )
        {
            if(keyInformation != null)
            {
                return true;
            }
            return false;
        }

        public static KeyInformation HasNotExpired(this KeyInformation keyInformation)
        {
            if (keyInformation != null)
            {
                TimeSpan ts = keyInformation.ExpirationDate - DateTime.Today;

                if (ts.Days >= 0)
                {
                    return keyInformation;
                }
            }
            return null;
        }
    }
}
