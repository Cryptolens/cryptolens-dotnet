using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SKM.V3
{
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
        public string OrderBy { get; set; }
        public string SearchQuery { get; set; }
    }

    public class GetKeysResult : BasicResult
    {
        public List<LicenseKey> LicenseKeys { get; set; }
        public int Returned { get; set; }
        public int Total { get; set; }
    }

    public class ActivateModel 
    {
        public int ProductId { get; set; }
        /// <summary>
        /// The Key Id, eg. 12345.
        /// </summary>
        public string Key { get; set; }
        public bool Sign { get; set; }

        public string MachineCode { get; set; }

        public int FieldsToReturn { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class KeyInfoResult : BasicResult
    {
        public LicenseKey LicenseKey { get; set; }
    }

    /// <summary>
    /// Input parameters to KeyLock method.
    /// </summary>
    public class KeyLockModel
    {
        /// <summary>
        /// The id of the product you want to access. You can find it
        /// when you are logged in on https://serialkeymanager.com/docs/api/v3/KeyLock
        /// and select the product in the drop down list.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The Key String, i.e. ABCDE-ABCDE-ABCDE-ABCDE.
        /// </summary>
        public string Key { get; set; }
    }
    /// <summary>
    /// The result of <see cref="SKM.KeyLock(AuthDetails, RemoveDataObjectModel)"/>
    /// </summary>
    public class KeyLockResult : BasicResult
    {
        public long KeyId { get; set; }
        public string Token { get; set; }

        /// <summary>
        /// Returns an <see cref="AuthDetails"/> which can be used 
        /// to authenticate a Web API 3 method.
        /// </summary>
        public AuthDetails GetAuthDetails()
        {
            return new AuthDetails
            {
                Token = Token,
                Version = 1
            };
        }
        /// <summary>
        /// Returns an <see cref="AuthDetails"/> which can be used 
        /// to authenticate a Web API 3 method.
        /// </summary>
        /// <param name="version">The API version</param>
        public AuthDetails GetAuthDetails(int version = 1)
        {
            return new AuthDetails
            {
                Token = Token,
                Version = version
            };
        }
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
        /// when using <see cref="SKM.SetIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// in which case it can be a signed int32, eg. 10, and -10 OR it is a the value that
        /// should be added to the current IntValue of an existing Data Object, in which case
        /// this value will be treated as an unsigned value, eg. 10 = -10. The latter case is
        /// relevant for <see cref="SKM.IncrementIntValue(AuthDetails, ChangeIntValueModel)"/>
        /// and <see cref="SKM.DecrementIntValue(AuthDetails, ChangeIntValueModel)"/>.
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
