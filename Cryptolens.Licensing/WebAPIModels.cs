using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Security.Cryptography;

namespace SKM.V3.Models
{

    /// <summary>
    ///   <para>
    ///     All the classes that are either used as a paramter for an API method or represent the
    ///     data that is being sent back.
    ///   </para>
    /// </summary>
    internal class NamespaceDoc
    {

    }

    public class CreateSessionResult : BasicResult
    {
        public string SessionId { get; set; }
    }
    public class CreateSessionModel
    {
        public int PaymentFormId { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }

        public string Heading { get; set; } // remember to set a max length.

        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string CustomField { get; set; }

        [MaxLength(1000)]
        public string Metadata { get; set; }

        public long Expires { get; set; }
    }

    public class GetMessagesResult : BasicResult
    {
        public List<MessageObject> Messages { get; set; }
    }

    public class GetMessagesModel
    {
        /// <summary>
        /// Specifies the channel, whose messages you would like to retrieve. If not set, messages from all channels will be returned.
        /// </summary>
        [DefaultValue("")]
        public string Channel { get; set; }

        /// <summary>
        /// Allows you to retrieve only those messages that were created after a certain Time (strictly greater than), eg. the last time you contacted the server. The format is unix timestamp. If no time is specified, all messages will be returned.
        /// </summary>
        [DefaultValue(0)]
        public long Time { get; set; }
    }

    public class RegisterEventModel
    {
        [DefaultValue(-1)]
        public int ProductId { get; set; }
        [DefaultValue("")]
        public string Key { get; set; }
        [DefaultValue("")]
        public string MachineCode { get; set; }
        [DefaultValue("")]
        public string FeatureName { get; set; }
        [DefaultValue("")]
        public string EventName { get; set; }

        [DefaultValue(0)]
        public int Value { get; set; }

        [DefaultValue("")]
        public string Currency { get; set; }

    }

    public class RegisterEventsModel
    {
        [DefaultValue(-1)]
        public int ProductId { get; set; }
        [DefaultValue("")]
        public string Key { get; set; }
        [DefaultValue("")]
        public string MachineCode { get; set; }
        [DefaultValue("")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string FeatureName { get; set; }
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string EventName { get; set; }
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Value { get; set; }
        [DefaultValue("")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Currency { get; set; }
        public long Time { get; set; }
    }
    public class GetCustomerLicensesModel
    {
        /// <summary>
        /// The id of the customer whose licenses we want to access.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Specifies the amount of parameters that should be included with each license key in the LiceseKeys. 
        /// If true, License Key will be used. By default, Basic License Key will be used (where for instance data objects and activated devices are omitted.)
        /// Please read more here: https://app.cryptolens.io/docs/api/v3/GetCustomerLicenses
        /// </summary>
        public bool Detailed { get; set; }


    }

    public class GetCustomerLicensesResult : BasicResult
    {
        public List<LicenseKey> LicenseKeys { get; set; }
    }

    public class RemoveCustomerModel
    {
        public int CustomerId { get; set; }
    }
    public class AddCustomerModel
    {
        /// <summary>
        /// The name of the customer (at most 100 chars)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email of the customer (at most 100 chars)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The company name of the company the customer belongs to (at most 100 chars)
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// If set to true, a portal link will be returned where the customer will be able to view their licenses.
        /// </summary>
        public bool EnableCustomerAssociation { get; set; }
    }

    public class AddCustomerResult : BasicResult
    {
        /// <summary>
        /// A unique integer identifier associated with this customer.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// A link that allows the customer to create an account where they will see their licenses (in the customer dashboard).
        /// </summary>
        public string PortalLink { get; set; }
    }
    public class DeactivateModel : KeyLockModel
    {
        [DefaultValue("")]
        public string MachineCode { get; set; }
    }

    public class KeyInfoModel
    {
        public int ProductId { get; set; }
        /// <summary>
        /// The Key Id, eg. 12345.
        /// </summary>
        public string Key { get; set; }

        public bool Sign { get; set; }
        public int FieldsToReturn { get; set; }

        public bool Metadata { get; set; }
    }

    public class CreateTrialKeyModel
    {
        /// <summary>
        /// The id of the product you want to access. You can find it
        /// when you are logged in on https://app.cryptolens.io/docs/api/v3/KeyLock
        /// and select the product in the drop down list.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The machine code (a string that identifies a device) that this trial key will be locked to.
        /// </summary>
        public string MachineCode { get; set; }

    }
    public class CreateKeyModel
    {
        public int ProductId { get; set; }
        public int Period { get; set; }
        public bool F1 { get; set; }
        public bool F2 { get; set; }
        public bool F3 { get; set; }
        public bool F4 { get; set; }
        public bool F5 { get; set; }
        public bool F6 { get; set; }
        public bool F7 { get; set; }
        public bool F8 { get; set; }
        [DefaultValue("")]
        public string Notes { get; set; }
        public bool Block { get; set; }
        public int CustomerId { get; set; } // maybe int instead?
        public bool TrialActivation { get; set; }
        public bool AutomaticActivation { get; set; }
        public int MaxNoOfMachines { get; set; }
        public string AllowedMachines { get; set; }
    }
    public class CreateKeyResult : BasicResult
    {
        public string Key { get; set; }
    }
    public class GetKeysModel
    {
        /// <summary>
        /// The product id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// If there are more than 100 keys, only 99 will be returned
        /// on the first page. in order to obtain the remaining licenses, 
        /// increment this parameter by 1.
        /// </summary>
        [DefaultValue(1)]
        public int Page { get; set; }
        /// <summary>
        /// Specifies the way to order the result. The order by field has the
        /// following structure: fieldName [ascending|descending]. For example, 
        /// If you want to order by the feature field 1 (F1), you should use F1.
        /// If you want it in descending order, please add the descending keywords 
        /// right after the field, eg. F1 descending. The ascending keyword is 
        /// the default, hence optional.
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Sorts the result so that only the license keys that satisfy the
        /// criterion will be displayed. Please see:
        /// https://support.serialkeymanager.com/kb/searching-for-licenses-using-linq-queries
        /// </summary>
        public string SearchQuery { get; set; }
    }

    public class GetKeysResult : BasicResult
    {
        /// <summary>
        /// A list of <see cref="LicenseKey"/> objects.
        /// </summary>
        public List<LicenseKey> LicenseKeys { get; set; }
        /// <summary>
        /// The number of licenses returned in the request, eg. size
        /// of the returned list.
        /// </summary>
        public int Returned { get; set; }
        /// <summary>
        /// The total number of keys available that satisfy the condition.
        /// For example, if search query is empty, the total is the number
        /// of license keys in the entire product. Otherwise, it's the 
        /// number of results of that query. By default, only 99 license 
        /// keys will be returned in a single request. There may still 
        /// be more license keys, which are obtained by increasing the 
        /// Page parameter.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Since not all keys will be returned if the number of them is
        /// more than 99, you can increment the Page parameter to list
        /// the remaining ones. This value is the limit of the number of
        /// pages available (this makes it easier to iterate through all
        /// the keys).
        /// </summary>
        public int PageCount { get; set; }
    }

    public class ActivateModel 
    {
        /// <summary>
        /// The product id, which can be found on the product page.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The Key string, eg. AAAA-BBBB-CCCC-DDDD.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// If true, the information inside the LiceseKey object will be signed. Note,
        /// in almost all cases, you should set this to True.
        /// </summary>
        public bool Sign { get; set; }

        /// <summary>
        /// The machine code (a string that identifies a device) for activation.	
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// An integer that allows you to restrict the information returned in the license key data object.
        /// Please read https://app.cryptolens.io/docs/api/v3/Activate#remarks for more details.
        /// </summary>
        public int FieldsToReturn { get; set; }

        /// <summary>
        /// Includes additional information about the license key, such as number of activated devices, etc.
        /// </summary>
        public bool Metadata { get; set; }

        /// <summary>
        /// When set to something greater than zero, floating licensing will be enabled.
        /// The time interval is then used to check that no more than the allowed number
        /// of machine codes (specified in maximumNumberOfMachines) have been activated
        /// in that time period (in seconds).
        /// </summary>
        public int FloatingTimeInterval { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class KeyInfoResult : BasicResult
    {
        public LicenseKey LicenseKey { get; set; }

        /// <summary>
        /// Metadata related to the license key. Note, this will not always contain value.
        /// Please see remarks for more information.
        /// </summary>
        /// <remarks>
        /// In order to access this variable, you need to set 
        /// <see cref="ActivateModel.Metadata"/> or <see cref="KeyInfoModel.Metadata"/> to true.
        /// Keep in mind that if your access token is restricted using the 'feature lock' field
        /// or if <see cref="ActivateModel.FieldsToReturn"/> or <see cref="KeyInfoModel.FieldsToReturn"/>
        /// (depending on if you activated or validated the license) have a certain value, metadata may still be null.
        /// </remarks>
        public KeyMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Metadata related to the license key. Note, this will not always contain value.
    /// </summary>
    public class KeyMetadata
    {
        /// <summary>
        /// The number of activated machines for this license key.
        /// </summary>
        public int ActivatedMachines { get; set; }

        /// <summary>
        /// Additional information about the license key. This variable will use the
        /// 'feature definitions' (that you can define in the dashboard for a given product)
        /// and the license properties to determine if the license is valid or not (eg. if it has expired).
        /// </summary>
        public LicenseStatus LicenseStatus { get; set; }

        /// <summary>
        /// The Signature of the metadata object.
        /// </summary>
        public string Signature { get; set; }

#if (NET46 || NETSTANDARD2_0)
        /// <summary>
        /// Verifies the integrity of the object (eg. it has not been since it was generated on the server).
        /// </summary>
        /// <param name="RSAPublicKey">Your public key (see this page https://app.cryptolens.io/docs/api/v3/QuickStart)</param>
        /// <returns>True if the signature is correct and false otherwise.</returns>
        public bool VerifySignature(string RSAPublicKey)
        {
            var rsa = RSA.Create();
            rsa.FromXmlString(RSAPublicKey);

            var res = Internal.SecurityMethods.VerifyObject(Signature, rsa);
            if (res == null)
                return false;

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<KeyMetadata>(res.Object);

            if (obj.ActivatedMachines != ActivatedMachines ||
                !obj.LicenseStatus.Equals(LicenseStatus))
                return false;

            return true;
        }
#endif
    }

    public class MachineLockLimit : KeyLockModel
    {
        public int NumberOfMachines { get; set; }
    }


    public class ChangeNotesModel : KeyLockModel
    {
        public string Notes { get; set; }
    }



    /// <summary>
    /// Input parameters to KeyLock method.
    /// </summary>
    public class KeyLockModel
    {
        /// <summary>
        /// The id of the product you want to access. You can find it
        /// when you are logged in on https://app.cryptolens.io/docs/api/v3/KeyLock
        /// and select the product in the drop down list.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The Key String, i.e. ABCDE-ABCDE-ABCDE-ABCDE.
        /// </summary>
        public string Key { get; set; }
    }
    /// <summary>
    /// The result of <see cref="SKM.V3.Methods.Auth.KeyLock(string, KeyLockModel)/>
    /// </summary>
    public class KeyLockResult : BasicResult
    {
        public long KeyId { get; set; }
        public string Token { get; set; }

    }

    public class AddDataObjectToKeyModel : KeyLockModel
    {
        /// <summary>
        /// The name of the data object. Max 10 characters.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A string value (text) to store. Max 10000 characters.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// An int value (int32) to store.
        /// </summary>
        public int IntValue { get; set; }
    }


    public class ListDataObjectsToKeyModel : KeyLockModel
    {
        /// <summary>
        /// Lists the names that contain the desired string only.
        /// </summary>
        [DefaultValue("")]
        public string Contains { get; set; }
    }


    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeStringValueToKeyModel : KeyLockModel, IChangeValueModel
    {
        public long Id { get; set; }
        public string StringValue { get; set; }
    }

    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeIntValueToKeyModel : KeyLockModel, IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.	
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// This is either the new int value that should be assigned to the Data Object
        /// when using <see cref="Data.SetIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// in which case it can be a signed int32, eg. 10, and -10 OR it is a the value that
        /// should be added to the current IntValue of an existing Data Object, in which case
        /// this value will be treated as an unsigned value, eg. 10 = -10. The latter case is
        /// relevant for <see cref="Data.IncrementIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// and <see cref="Data.DecrementIntValue(AuthDetails, ChangeIntValueModel)"/>.
        /// </summary>
        public int IntValue { get; set; }

        /// <summary>
        /// If set to true, it will be possible to specify an upper/lower bound. 
        /// (for Increment Int Value) For example, if you set the Bound parameter (below) to 10, you
        /// will be able to increment the int value until you reach ten (inclusive).
        /// Once the upper bound is reached, an error will be thrown.
        /// (for Decrement Int Value) For example, if you set the Bound parameter (below) to 0, 
        /// you will be able to decrement the int value until you reach zero (inclusive).
        /// Once the lower bound is reached, an error will be thrown.
        /// </summary>
        public bool EnableBound { get; set; }

        /// <summary>
        /// This is the upper/lower bound that will be enforced on the increment or
        /// decrement operation. It will only be enforced if EnableBound
        /// is set to true. Please read the description above.
        /// </summary>
        public int Bound { get; set; }
    }


    /// <summary>
    /// Used to remove a data object.
    /// </summary>
    public class RemoveDataObjectToKeyModel : KeyLockModel, IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.
        /// </summary>
        public long Id { get; set; }
    }



    internal interface IAddOrListDataObjectsModel
    {
        /// <summary>
        /// Indicates if the data object should be added to a
        /// license key, a product or the entire user account. 
        /// <see cref="DataObject"/>
        /// </summary>
        DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the Referencer. It can either be an id to a product that
        /// you have or to a license key. When ReferencerType is set to User,
        /// there is no need to set this value.
        /// </summary>
        int ReferencerId { get; set; }
    }

    /// <summary>
    /// Used to add a new Data Object.
    /// </summary>
    public class AddDataObjectModel : IAddOrListDataObjectsModel
    {
        /// <summary>
        /// The name of the data object. Max 10 characters.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A string value (text) to store. Max 100 characters.
        /// </summary>
        public string StringValue { get; set; }
        /// <summary>
        /// An int value (int32) to store.
        /// </summary>
        public int IntValue { get; set; }

        /// <summary>
        /// Indicates if the data object should be added to a
        /// license key, a product or the entire user account. 
        /// <see cref="DataObject"/>
        /// </summary>
        public DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the Referencer. It can either be an id to a product
        /// that you have or to a license key. When ReferencerType is set
        /// to User, there is no need to set this value.
        /// </summary>
        public int ReferencerId { get; set; }

    }

    /// <summary>
    /// Used to list Data Objects.
    /// </summary>
    public class ListDataObjectsModel : IAddOrListDataObjectsModel
    {

        /// <summary>
        /// Indicates if the data object is associated with a license key, 
        /// a product or the entire user account. User = 0, Product = 1, Key = 2.
        /// </summary>
        public DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the Referencer. It can either be an id to a product that
        /// you have or to a license key. When ReferencerType is set to User,
        /// there is no need to set this value.
        /// </summary>
        public int ReferencerId { get; set; }

        /// <summary>
        /// Shows only Data Objects where the name contains the following string.
        /// </summary>
        public string Contains { get; set; }

        /// <summary>
        /// If set to true, all data objects will be returned, that is, 
        /// both those associated with your entire account, a specific
        /// product and a license key. In addition, each data object
        /// item will include the ReferencerType and its Id. Otherwise,
        /// i.e. when set to false, only the data objects associated 
        /// with the user, product or key will be returned, without the 
        /// ReferencerType and its Id.
        /// </summary>
        public bool ShowAll { get; set; }
    }



    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    internal interface IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.
        /// </summary>
        long Id { get; set; }
    }

    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeStringValueModel : IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// A string value (text) to store. Max 100 characters.
        /// </summary>
        public string StringValue { get; set; }
    }

    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeIntValueModel : IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// This is either the new int value that should be assigned to the Data Object
        /// when using <see cref="Data.SetIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// in which case it can be a signed int32, eg. 10, and -10 OR it is a the value that
        /// should be added to the current IntValue of an existing Data Object, in which case
        /// this value will be treated as an unsigned value, eg. 10 = -10. The latter case is
        /// relevant for <see cref="Data.IncrementIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// and <see cref="Data.DecrementIntValue(AuthDetails, ChangeIntValueModel)"/>.
        /// </summary>
        public int IntValue { get; set; }


        /// <summary>
        /// If set to true, it will be possible to specify an upper/lower bound. 
        /// (for Increment Int Value) For example, if you set the Bound parameter (below) to 10, you
        /// will be able to increment the int value until you reach ten (inclusive).
        /// Once the upper bound is reached, an error will be thrown.
        /// (for Decrement Int Value) For example, if you set the Bound parameter (below) to 0, 
        /// you will be able to decrement the int value until you reach zero (inclusive).
        /// Once the lower bound is reached, an error will be thrown.
        /// </summary>
        public bool EnableBound { get; set; }

        /// <summary>
        /// This is the upper/lower bound that will be enforced on the increment or
        /// decrement operation. It will only be enforced if EnableBound
        /// is set to true. Please read the description above.
        /// </summary>
        public int Bound { get; set; }
    }

    /// <summary>
    /// Used to remove a data object.
    /// </summary>
    public class RemoveDataObjectModel : IChangeValueModel
    {
        /// <summary>
        /// The unique object id for the data object.
        /// </summary>
        public long Id { get; set; }
    }



    /// <summary>
    /// Stores the parameters that are required by the Extend License method.
    /// </summary>
    public class ExtendLicenseModel
    {
        /// <summary>
        /// The id of the product you want to access. You can find it
        /// when you are logged in on https://serialkeymanager.com/docs/api/v3/ExtendLicense
        /// and select the product in the drop down list.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The Key String, i.e. ABCDE-ABCDE-ABCDE-ABCDE.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The number of days the license should be extended.
        /// </summary>
        public int NoOfDays { get; set; }
    }

    /// <summary>
    /// Stores the parameters required by AddFeature and RemoveFeature methods.
    /// </summary>
    public class FeatureModel
    {
        public int ProductId { get; set; }
        /// <summary>
        /// The Key String, i.e. ABCDE-ABCDE-ABCDE-ABCDE.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// From 1 to 8.
        /// </summary>
        public int Feature { get; set; }
    }



    /// <summary>
    /// Used to return the list of <see cref="DataObject"/>
    /// </summary>
    public class ListOfDataObjectsResult : BasicResult
    {
        /// <summary>
        /// A list of data objects, where each data object is of the type
        /// <see cref="DataObject"/>. Note, when <see cref="ListDataObjectsModel.ShowAll"/>
        /// is set to true, the data objects will contain additional information about the
        /// referencer. So, when <see cref="ListDataObjectsModel.ShowAll"/>, you can convert
        /// the <see cref="DataObject"/> to a <see cref="DataObjectWithReferencer"/> using
        /// implicit or explicit conversion.
        /// </summary>
        public List<DataObject> DataObjects { get; set; }
    }


    /// <summary>
    /// Used to return the list of <see cref="DataObjectWithReferencer"/> 
    /// for internal purposes.
    /// </summary>
    internal class ListOfDataObjectsResultWithReferencer : BasicResult
    {
        public List<DataObjectWithReferencer> DataObjects { get; set; }
    }


    /// <summary>
    /// Used to return the object Id of a Data Object.
    /// </summary>
    public class DataObjectIdResult : BasicResult
    {
        /// <summary>
        /// The unique object id for the new data object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// A simple result that tells if a request is successful,
    /// and optionally provides a message.
    /// </summary>
    public class BasicResult
    {
        /// <summary>
        /// Tells whether the result is successful or unsuccessful.
        /// </summary>
        public ResultType Result { get; set; }

        /// <summary>
        /// The message that provides additional information about the result.
        /// If it's a successful result, null will be returned. Otherwise,
        /// in case of an error, a short message will be returned describing the error.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Indicates whether a result was successful or unsuccessful.
    /// </summary>
    public enum ResultType
    {
        Success,
        Error
    }

    /// <summary>
    /// Stores information that is required to identify yourself to SKM.
    /// This includes the Token and Version.
    /// </summary>
    [Serializable]
    public class AuthDetails
    {
        /// <summary>
        /// This token helps SKM to identify you and ensure that
        /// you have the required permission. Read more here 
        /// https://serialkeymanager.com/docs/api/v3/Auth.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The version of the method you want to access. By default,
        /// it's not needed, however, if you would like to access
        /// a newer version, it can be specified. You can read more here
        /// https://serialkeymanager.com/docs/api/v3/Versioning
        /// </summary>
        public int Version { get; set; }
    }
}
