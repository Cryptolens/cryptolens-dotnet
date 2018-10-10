using System;
using System.Collections.Generic;
using System.Text;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Payment form related methods.
    /// </summary>
    public class PaymentForm
    {
        /// <summary>
        /// This method will create a new session for a Payment Form.
        /// It allows you to customize appearance of the form (such as price, heading, etc).
        /// You should only create new sessions from a server side (i.e. never directly from your application). 
        /// Note, session will only work once and it will eventually expire depending on Expires parameter.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CreateSessionResult CreateSession(string token, CreateSessionModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateSessionResult>(parameters, "/paymentform/createsession/", token);
        }
    }
}
