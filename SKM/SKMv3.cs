using SKGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3
{
    public static class Key
    {
        public static BasicResult Activate(ActivateModel parameters, AuthDetails auth)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/activate/", auth.Token);
        }
         
    }
}
