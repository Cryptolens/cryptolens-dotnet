using SKM.V3.Internal;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Methods
{

    /// <summary>
    /// <para>
    /// Methods that are used to call the Web API 3. A detailed list is available at
    /// <a href="https://app.cryptolens.io/docs/api/v3">https://app.cryptolens.io/docs/api/v3</a>.
    /// Each class contains a list of related methods that perform an action on a certain object. For example, the 
    /// <see cref="Key"/> class contains the methods <see cref="Key.Activate(string, ActivateModel)"/>
    /// and <see cref="Key.CreateKey(string, CreateKeyModel)"/>, and they are related since the affect a
    /// 'Key' object (<see cref="LicenseKey"/> in SKM Client API). If you want to change a customer object
    /// (<see cref="Customer"/>), such as <see cref="CustomerMethods.AddCustomer(string, AddCustomerModel)"/> 
    /// or <see cref="CustomerMethods.GetCustomerLicenses(string, GetCustomerLicensesModel)"/>,
    /// you would have to look into the <see cref="CustomerMethods"/> class.
    /// </para>
    /// </summary>
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Methods that related to authentication. A complete list
    /// can be found here: https://app.cryptolens.io/docs/api/v3/Auth
    /// </summary>
    public class AI
    {
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
        public static KeyLockResult RegisterEvent(string token, KeyLockModel parameters)
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
        public static KeyLockResult RegisterEvents(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(parameters, "/ai/registerevents/", token);
        }
    }
}
