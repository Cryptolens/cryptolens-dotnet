﻿using SKGL;
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
    /// can be found here: https://app.cryptolens.io/docs/api/v3/Key
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
        /// https://app.cryptolens.io/docs/api/v3/Activate
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
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
            if (parameters != null)
            {
                if (parameters.OSInfo == null)
                {
                    try
                    {
                        parameters.OSInfo = Helpers.GetOSStats();
                    }
                    catch { }
                }
                else if (parameters.OSInfo == "")
                {
                    parameters.OSInfo = null;
                }
            }
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/activate/", token, modelVersion: 3);
        }


        /// <summary>
        /// This method is similar to <see cref="Key.Activate(string, ActivateModel)"/> with the only exception that it will return a license key signed with the new protocol.
        /// <b>Note:</b> it's better to use this method, especially if you target Mono/Unity.<br/>
        /// In order to get the license key, you can call <see cref="LicenseKey.FromResponse(string, RawResponse)"/>.
        /// </summary>
        public static RawResponse Activate(string token, int productId, string key, string machineCode = "", bool metadata = false,
            int floatingTimeInterval = 0, int maxOverdraft = 0, string OSInfo = null, string friendlyName = "", string LicenseServerUrl = null)
        {

            var parameters = new ActivateModel()
            {
                ProductId = productId,
                Key = key,
                MachineCode = machineCode,
                Metadata = metadata,
                FloatingTimeInterval = floatingTimeInterval,
                MaxOverdraft = maxOverdraft,
                Sign = true,
                SignMethod = SignMethod.StringSign,
                OSInfo = OSInfo,
                FriendlyName = friendlyName,
                LicenseServerUrl = LicenseServerUrl
            };

            if (parameters != null)
            {
                if (parameters.OSInfo == null)
                {
                    try
                    {
                        parameters.OSInfo = Helpers.GetOSStats();
                    }
                    catch { }
                }
                else if (parameters.OSInfo == "")
                {
                    parameters.OSInfo = null;
                }
            }

            var res = HelperMethods.SendRequestToWebAPI3<RawResponse>(parameters, "/key/activate/", token, modelVersion: 3);
            return res;
        }

        /// <summary>
        /// This method is similar to <see cref="Key.GetKey(string, KeyInfoModel)"/> with the only exception that it will return a license key signed with the new protocol.
        /// <b>Note:</b> it's better to use this method, especially if you target Mono/Unity.<br/>
        /// In order to get the license key, you can call <see cref="LicenseKey.FromResponse(string, RawResponse)"/>.
        /// </summary>
        public static RawResponse GetKey(string token, int productId, string key, bool metadata = false
            , string LicenseServerUrl = null)
        {

            var parameters = new KeyInfoModel()
            {
                ProductId = productId,
                Key = key,
                Metadata = metadata,
                Sign = true,
                SignMethod = SignMethod.StringSign,
                LicenseServerUrl = LicenseServerUrl
            };

            var res = HelperMethods.SendRequestToWebAPI3<RawResponse>(parameters, "/key/getkey/", token, modelVersion: 3);
            return res;
        }

        /// <summary>
        ///This method will 'undo' a key activation with a certain machine code. 
        ///The key should not be blocked, since otherwise this method will throw an error.
        /// https://app.cryptolens.io/docs/api/v3/Deactivate
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
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
            if (parameters != null)
            {
                if (parameters.OSInfo == null)
                {
                    try
                    {
                        parameters.OSInfo = Helpers.GetOSStats();
                    }
                    catch { }
                }
                else if (parameters.OSInfo == "")
                {
                    parameters.OSInfo = null;
                }
            }
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/deactivate/", token);
        }

        /// <summary>
        /// This method will change a given feature to be true (in a license).
        /// If the key algorithm in the product is SKGL, the key string 
        /// will be changed if necessary. Otherwise, if SKM15 is used, 
        /// the key will stay the same.
        /// If the key is changed, the new key will be stored in the message.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
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
        /// <example>
        /// <code language="vb" title="Creating a new key">
        /// Private Sub CreateKey()
        ///     Dim parameters = New CreateKeyModel() With {
        ///         .ProductId = 3,
        ///         .F1 = 1,
        ///         .Period = 30
        ///     }
        ///     Dim auth = "{access token with CreateKey permission and optional product lock}"
        ///     Dim result = Key.CreateKey(token:=auth, parameters:=parameters)
        ///     If (result IsNot Nothing AndAlso result.Result = ResultType.Success) Then
        ///         ' successful
        ///         Console.WriteLine(result.Key)
        ///     End If
        /// End Sub
        /// </code>
        /// </example>
        public static CreateKeyResult CreateKey(string token, CreateKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateKeyResult>(parameters, "/key/createkey/", token);
        }

        /// <summary>
        /// This method creates a license key that is time-limited, node-locked and with the "Time-Limited"
        /// and "Trial" features set to true (which can be set by editing the feature definitions on the product page).
        /// Note, by default, the trial will work for 15 days. To change this limit, you can set the Feature Lock
        /// to the desired value, when creating the access token.
        /// <br>
        /// If a trial key was already created for a certain machine code, this method will try to find the license key
        /// and return it instead. However, this will only occur if the license key is still a trial key (based on feature
        /// definitions) and is not blocked.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>

        public static CreateKeyResult CreateTrialKey(string token, CreateTrialKeyModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateKeyResult>(parameters, "/key/createtrialkey/", token);
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
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/getkey/", token, modelVersion: 3);
        }


        /// <summary>
        /// This method will block a specific license key to ensure that the key cannot be
        /// accessible by most of the methods in the Web API (activation, validation, 
        /// optional field, and deactivation).Note, blocking the key will still allow you 
        /// to access the key in Web API 3, unless otherwise stated for a given Web API 3 method. 
        /// To do the reverse, please see <see cref="UnblockKey(string, KeyLockModel)"/>.
        /// </summary>
        /// <param name="token">The access token (https://app.cryptolens.io/User/AccessToken#/) with Block key permission.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult BlockKey(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/blockkey/", token);
        }

        /// <summary>
        /// This method will unblock a specific license key to ensure that the key can be
        /// accessible by most of the methods in the Web API (activation, validation,
        /// optional field, and deactivation). To do the reverse, please see <see cref="BlockKey(string, KeyLockModel)"/>.
        /// </summary>
        /// <param name="token">The access token (https://app.cryptolens.io/User/AccessToken#/) with Unblock key permission.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult UnblockKey(string token, KeyLockModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/unblockkey/", token);
        }


        /// <summary>
        /// This method will change the maximum number of machine codes that a license key can have.
        /// </summary>
        /// <param name="token">Details such as Token and Version.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <remarks>This method may, in rare cases, return null if an error has occurred.
        /// Null should be seen as an unsuccessful result.
        /// </remarks>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult MachineLockLimit(string token, MachineLockLimit parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<KeyInfoResult>(parameters, "/key/machinelocklimit/", token);
        }

        /// <summary>
        /// This method will change the content of the notes field of a given license key.
        /// </summary>
        /// <param name="token">The access token (https://app.cryptolens.io/User/AccessToken#/) with Change Notes permission.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult ChangeNotes(string token, ChangeNotesModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/changenotes/", token);
        }


        /// <summary>
        /// This method will change the reseller of a license.
        /// If the reseller is not specified (for example, if ResellerId=0)
        /// or the reseller with the provided ID does not exist, any reseller
        /// that was previously associated with the license will be dissociated.
        /// </summary>
        /// <param name="token">The access token (https://app.cryptolens.io/User/AccessToken#/) with Change Reseller permission.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult ChangeReseller(string token, ChangeResellerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/changereseller/", token);
        }

        /// <summary>
        /// This method will change the customer associated with a license.
        /// If the customer is not specified (for example, if CustomerId=0) or
        /// the customer with the provided ID does not exist, any customer that
        /// was previously associated with the license will be dissociated.
        /// </summary>
        /// <param name="token">The access token (https://app.cryptolens.io/User/AccessToken#/) with Change Customer permission.</param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A BasicResult object or null.</returns>
        public static BasicResult ChangeCustomer(string token, ChangeCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/key/changecustomer/", token);
        }
    }
}
