using SKGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
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
        /// <example>
        /// Assuming that you've created a key in the SKM platform that has maximum number of machines set to anything greater than zero, we can run the following code:
        /// <code language="csharp" title="Activation example">
        /// var auth = "{access token with permission to access the activate method}"
        /// var result = Key.Activate(token: auth, parameters: new ActivateModel()
        /// {
        ///     Key = "GEBNC-WZZJD-VJIHG-GCMVD",
        ///     ProductId = 3349,
        ///     Sign = true,
        ///     MachineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
        /// });
        ///
        /// if(result == null || result.Result == ResultType.Error)
        /// {
        ///     // an error occured or the key is invalid or it cannot be activated
        ///     // (eg. the limit of activated devices was achieved)
        /// }
        /// // everything went fine if we are here!
        /// </code>
        /// </example>
        public static KeyInfoResult Activate(string token, ActivateModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/activate/", token);
        }

        /// <summary>
        ///This method will 'undo' a key activation with a certain machine code. 
        ///The key should not be blocked, since otherwise this method will throw an error.
        /// https://serialkeymanager.com/docs/api/v3/Deactivate
        /// </summary>
        /// <param name="token">The access token. Read more at https://serialkeymanager.com/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="BasicResult"/> or null.</returns>
        /// <example>
        /// <code language="csharp" title="Deactivation example">
        /// var auth = "{access token with permission to access the deactivate method}"
        /// var result = Key.Deactivate(token: auth, parameters: new DeactivateModel() 
        /// {
        ///         Key = "GEBNC-WZZJD-VJIHG-GCMVD", 
        ///         ProductId = 3349,
        ///         MachineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
        /// });
        /// 
        /// if(result == null || result.Result == ResultType.Error)
        /// {
        ///     // could not deactivate. maybe it has already been deactivated.
        ///     // more information can be found in the message.
        /// }
        /// 
        /// // everything went fine if we are here!
        /// </code>
        /// </example>
        public static BasicResult Deactivate(string token, DeactivateModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/deactivate/", token);
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
        /// <param name="token">The access token</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <example>
        /// Here is an example that demonstrates the use of the method.
        /// <code language="cs" title="C#">
        /// var keydata = new FeatureModel() { Key = "LXWVI-HSJDU-CADTC-BAJGW", Feature = 2, ProductId = 3349 };
        /// var auth = "WyI2Iiwib3lFQjFGYk5pTHYrelhIK2pveWdReDdEMXd4ZDlQUFB3aGpCdTRxZiJd";
        ///
        /// var result = Key.RemoveFeature(auth, keydata);
        ///
        /// if (result != null && result.Result == ResultType.Success)
        /// {
        ///     // feature 2 is set to true.
        /// }
        /// else
        /// {
        ///     Assert.Fail();
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
        /// var keydata = new ExtendLicenseModel() { Key = "ITVBC-GXXNU-GSMTK-NIJBT", NoOfDays = 30, ProductId = 3349 };
        /// var auth = "WyI0IiwiY0E3aHZCci9FWFZtOWJYNVJ5eTFQYk8rOXJSNFZ5TTh1R25YaDVFUiJd";
        ///
        /// var result = Key.ExtendLicense(auth, keydata);
        ///
        /// if (result != null && result.Result == ResultType.Success)
        /// {
        ///
        ///     // the license was successfully extended with 30 days.
        /// }
        /// else
        /// {
        ///     Assert.Fail();
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
        /// This method will return information about a license key, similar to 
        /// Validate [Web API 2]. In contrast to activation, this method (aka Key Validation)
        /// will be in read only mode. That is, it will not add a device to the license nor 
        /// use trial activation. More about this in Remarks.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static KeyInfoResult GetKey(string token, KeyInfoModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/getkey/", token);
        }


        /// <summary>
        /// This method will block a specific license key to ensure that the key cannot be
        /// accessible by most of the methods in the Web API (activation, validation, 
        /// optional field, and deactivation).Note, blocking the key will still allow you 
        /// to access the key in Web API 3, unless otherwise stated for a given Web API 3 method. 
        /// To do the reverse, please see <see cref="UnblockKey(string, KeyLockModel)"/>.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static KeyInfoResult BlockKey(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/blockkey/", token);
        }

        /// <summary>
        /// This method will unblock a specific license key to ensure that the key can be
        /// accessible by most of the methods in the Web API (activation, validation,
        /// optional field, and deactivation). To do the reverse, please see <see cref="BlockKey(string, KeyLockModel)"/>.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static KeyInfoResult UnblockKey(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/unblockkey/", token);
        }
    }
}
