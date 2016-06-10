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
    public static class Key
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
        public static KeyInfoResult Activate(string token, ActivateModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/activate/", token);
        }

        /// <summary>
        /// This method will change a given feature to be true (in a license).
        /// If the key algorithm in the product is SKGL, the key string 
        /// will be changed if necessary. Otherwise, if SKM15 is used, 
        /// the key will stay the same.
        /// If the key is changed, the new key will be stored in the message.
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <example>
        /// Here is an example that demonstrates the use of the method.
        /// <code language="cs" title="C#">
        /// public void AddFeatureTest()
        /// {
        ///     var keydata = new FeatureModel() { Key = "LXWVI-HSJDU-CADTC-BAJGW", Feature = 2, ProductId = 3349 };
        ///     var auth = new AuthDetails() { Token = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd" };
        /// 
        ///     var result = SKM.AddFeature(auth, keydata);
        /// 
        ///     if (result != null &amp;&amp; result.Result == ResultType.Success)
        ///     {
        ///         // feature 2 is set to true.
        ///     }
        ///     else
        ///     {
        ///         Assert.Fail();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult AddFeature(string token, FeatureModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/addfeature/", token);
        }

        /// <summary>
        /// This method will change a given feature to be false (in a license).
        /// If the key algorithm in the product is SKGL, the key string 
        /// will be changed if necessary. Otherwise, if SKM15 is used, 
        /// the key will stay the same.
        /// If the key is changed, the new key will be stored in the message.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <example>
        /// Here is an example that demonstrates the use of the method.
        /// <code language="cs" title="C#">
        /// public void AddFeatureTest()
        /// {
        ///     var keydata = new FeatureModel() { Key = "LXWVI-HSJDU-CADTC-BAJGW", Feature = 2, ProductId = 3349 };
        ///     var auth = new AuthDetails() { Token = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd" };
        /// 
        ///     var result = SKM.RemoveFeature(auth, keydata);
        /// 
        ///     if (result != null &amp;&amp; result.Result == ResultType.Success)
        ///     {
        ///         // feature 2 is set to true.
        ///     }
        ///     else
        ///     {
        ///         Assert.Fail();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult RemoveFeature(string token, FeatureModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/removefeature/", token);
        }

        /// <summary>
        /// This method will extend a license by a certain amount of days. 
        /// If the key algorithm in the product is SKGL, the key string 
        /// will be changed if necessary. Otherwise, if SKM15 is used, 
        /// the key will stay the same.
        /// If the key is changed, the new key will be stored in the message.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <example>
        /// Here is an example that demonstrates the use of the method.
        /// <code language="cs" title="C#">
        /// public void ExtendLicenseExample()
        /// {
        ///    var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
        ///    var auth = new AuthDetails() { Token = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd" };
        ///
        ///    var result = SKM.ExtendLicense(auth, keydata);
        ///
        ///    if (result != null &amp;&amp; result.Result == ResultType.Success)
        ///    {
        ///        // the license was successfully extended with 30 days.
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult ExtendLicense(string token, ExtendLicenseModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/extendlicense/", token);
        }

        /// <summary>
        /// This method will create a new license key, which is the same as GenerateKey in Web API 2.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static CreateKeyResult CreateKey(string token, CreateKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateKeyResult>(parameters, "/key/createkey/", token);
        }


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
