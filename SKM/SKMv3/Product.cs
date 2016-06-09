using SKGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3
{
    /// <summary>
    /// Methods that perform operations on a license key. A complete list
    /// can be found here: https://serialkeymanager.com/docs/api/v3/Key
    /// </summary>
    public static class Product
    {
        /// <summary>
        /// This method will perform a key activation, similar to Activate [Web API 2]. 
        /// In contrast to key validation, key activation is not read only since it can 
        /// change license key data depending on configurations such as trial activation,
        /// etc. If trial activation is enabled, a key can be altered. Information that 
        /// is retrieved can be signed by the server to be able to keep validate keys 
        /// without Internet connection. Please keep in mind that the Feature lock can 
        /// be used to restrict the fields that can be shown in the result (fieldsToReturn). 
        /// More about this in Remarks. 
        /// https://serialkeymanager.com/docs/api/v3/Activate
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="BasicResult"/> or null.</returns>
        /// <remarks>
        /// The feature lock value is used to store the filedsToReturn value. If you set a certain value in the feature lock, it will be prioritized higher than the fieldsToReturn parameter.<br></br>
        /// • To compute the value of the feature lock, please use the Hide column, for those fields that you want to omit in the result above.<br></br>
        /// • If the ActivatedMachines is hidden, only the current machine code will be included(used during this particular activation). Otherwise, all machine codes will be included.
        /// </remarks>
        public static GetKeysResult GetKeys(string token, GetKeysModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetKeysResult>(parameters, "/product/getkeys/", token);
        }

        
    }
}
