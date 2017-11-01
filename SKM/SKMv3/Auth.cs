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
    /// <a href="https://serialkeymanager.com/docs/api/v3">https://serialkeymanager.com/docs/api/v3</a>.
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
    /// can be found here: https://serialkeymanager.com/docs/api/v3/Auth
    /// </summary>
    public class Auth
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
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/KeyLock">https://serialkeymanager.com/docs/api/v3/KeyLock</a> <br/>
        /// </remarks>
        /// <returns>Returns <see cref="KeyLockResult"/> or null.</returns>
        public static KeyLockResult KeyLock(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyLockResult>(parameters, "/auth/keylock/", token);
        }
    }
}
