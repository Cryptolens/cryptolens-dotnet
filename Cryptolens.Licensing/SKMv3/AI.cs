using SKM.V3.Internal;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that related to data gathering. A complete list
    /// can be found here: https://app.cryptolens.io/docs/api/v3/AI
    /// </summary>
    public class AI
    {
        /// <summary>
        /// This method will, given a license key, generate a new access token
        /// that is locked to that particular key and return the Id of that key.
        /// The scope of the access token is preserved (i.e. all methods that were
        /// enabled in the access token used to access this method will be copied
        /// to the new access token) except for the key lock, which is going to be
        /// changed to the id of the license key. Note, for this method to work,
        /// the access token used to access this method has to have key lock set
        /// to -1. All of these details are described in Remarks.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RegisterEvent">https://app.cryptolens.io/docs/api/v3/RegisterEvent</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/> or null.</returns>
        public static BasicResult RegisterEvent(string token, RegisterEventModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(parameters, "/ai/registerevent/", token);
        }

        /// <summary>
        /// This method will register an event that has occured in either the client app (eg. start of a certain feature or interaction within a feature) or in a third party provider (eg. a payment has occured, etc).
        /// Note: You can either use this method standalone(eg.by only providing a machine code/device identifier) or together with Cryptolens Licensing module(which requires productId and optionally keyid to be set). The more information that is provided, the better insights can be provided.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://app.cryptolens.io/docs/api/v3/RegisterEvent">https://app.cryptolens.io/docs/api/v3/RegisterEvent</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/> or null.</returns>
        public static BasicResult RegisterEvents(string token, RegisterEventsModel parameters)
        {
            var internalModel = new RegisterEventsModelServer
            {
                ProductId = parameters.ProductId,
                Events = Newtonsoft.Json.JsonConvert.SerializeObject(parameters.Events),
                Key = parameters.Key,
                MachineCode = parameters.MachineCode,
                LicenseServerUrl = parameters.LicenseServerUrl
            };

            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(internalModel, "/ai/registerevents/", token);
        }

        private class RegisterEventsModelServer: RequestModel
        {
            public int ProductId { get; set; }
            public string Key { get; set; }
            public string MachineCode { get; set; }
            public string Events { get; set; }
        }
    }
}
