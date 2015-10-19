namespace SKGL
{
    public class ExtendLicenseModel
    {
        public int ProductId { get; set; }
        /// <summary>
        /// The Key String, i.e. ABCDE-ABCDE-ABCDE-ABCDE.
        /// </summary>
        public string Key { get; set; }
        public int NoOfDays { get; set; }
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

    public enum ResultType
    {
        Success,
        Error
    }
}
