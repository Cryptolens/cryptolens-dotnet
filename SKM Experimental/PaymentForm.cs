using System;
using System.Collections.Generic;
using System.Text;

using Cryptolens.SKM.Models;
using Cryptolens.SKM.Helpers;

namespace Cryptolens.SKM.PaymentForms
{
    public class PaymentForm
    {
        public static CreateSessionResult CreateSession(CreateSessionModel inputParameters, string token)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateSessionResult>(inputParameters, "/paymentform/", token);
        }
    }
}
