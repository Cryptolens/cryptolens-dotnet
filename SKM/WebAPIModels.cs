using System.Collections.Generic;

namespace SKGL
{
    public interface IAddOrListDataObjectsModel
    {
        /// <summary>
        /// Same as Reference Type, i.e. a key, a product or a user. <see cref="DataObject"/>
        /// </summary>
        DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the product, key, or the user.
        /// </summary>
        /// <remarks>
        /// NOTE: In future, we might want to change to long
        /// as Key already uses long for identification.
        /// </remarks>
        int ReferencerId { get; set; }
    }
    public class AddDataObjectModel : IAddOrListDataObjectsModel
    {
        public string Name { get; set; }
        public string StringValue { get; set; }
        public int IntValue { get; set; }

        /// <summary>
        /// Same as Reference Type, i.e. a key, a product or a user. <see cref="DataObject"/>
        /// </summary>
        public DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the product, key, or the user.
        /// </summary>
        /// <remarks>
        /// NOTE: In future, we might want to change to long
        /// as Key already uses long for identification.
        /// </remarks>
        public int ReferencerId { get; set; }

    }

    public class ListDataObjectsModel : IAddOrListDataObjectsModel
    {

        /// <summary>
        /// Same as Reference Type, i.e. a key, a product or a user. <see cref="DataObject"/>
        /// </summary>
        public DataObjectType ReferencerType { get; set; }

        /// <summary>
        /// The id of the product, key, or the user.
        /// </summary>
        /// <remarks>
        /// NOTE: In future, we might want to change to long
        /// as Key already uses long for identification.
        /// </remarks>
        public int ReferencerId { get; set; }

        /// <summary>
        /// Lists the names that contain the desired string only.
        /// </summary>
        public string Contains { get; set; }

        /// <summary>
        /// Show all data objects for the current user.
        /// </summary>
        public bool ShowAll { get; set; }
    }



    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public interface IChangeValueModel
    {
        long Id { get; set; }
    }

    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeStringValueModel : IChangeValueModel
    {
        public long Id { get; set; }

        public string StringValue { get; set; }
    }

    /// <summary>
    /// Used to identify a data object.
    /// </summary>
    public class ChangeIntValueModel : IChangeValueModel
    {
        public long Id { get; set; }

        public int IntValue { get; set; }
    }

    /// <summary>
    /// Used to remove a data object.
    /// </summary>
    public class RemoveDataObjectModel : IChangeValueModel
    {
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
        public List<DataObject> DataObjects { get; set; }
    }

    /// <summary>
    /// Used to return the object Id of a Data Object.
    /// </summary>
    public class DataObjectIdResult : BasicResult
    {
        public long Id { get; set; }
    }

    /// <summary>
    /// A simple result that tells if a request is successful,
    /// and optionally provides a message.
    /// </summary>
    public class BasicResult
    {
        public ResultType Result { get; set; }
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
