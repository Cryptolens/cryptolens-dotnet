using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKM.V3.Internal;
using SKM.V3.Models;

namespace SKM.V3.Methods
{
    /// <summary>
    /// These methods are related to the <a href="https://help.cryptolens.io/recurring-payments/index">recurring billing</a> module.
    /// They can all be accessed using an access token with 'Subscription' permission.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// This method records uses Stripe's metered billing to record usage for a certain subscription.
        /// In order to use this mehtod, you need to have set up recurring billing. A record will be created using Stripe's API with action set to 'increment'.
        /// https://app.cryptolens.io/docs/api/v3/RecordUsage
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="BasicResult"/> or null.</returns>
        public static BasicResult RecordUsage(string token, RecordUsageModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/subscription/recordusage/", token);
        }
    }
}
