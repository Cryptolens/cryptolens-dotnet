namespace SKGL
{
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
