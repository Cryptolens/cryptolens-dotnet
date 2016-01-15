
namespace SKGL
{
    /// <summary>
    /// A Data Object used to store information.
    /// </summary>
    public class DataObject
    {
        public long Id { get; set; }

        /// <summary>
        /// A way to identify the current object, for instance, OS_Version.
        /// </summary>
        /// <remarks>Max 10 chars.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Max 100 chars.
        /// </summary>
        public string StringValue { get; set; }

        public int IntValue { get; set; }
    }

    /// <summary>
    /// This class adds some more fields that tell us which referencer this data object
    /// belongs to.
    /// </summary>
    public class DataObjectWithReferencer : DataObject
    {
        /// <summary>
        /// The id of the product, key, or the user.
        /// </summary>
        public int ReferencerId { get; set; }

        /// <summary>
        /// Specifies what this data object should be associated to, eg. a Key, a Product or the entire user account.
        /// </summary>
        public DataObjectType ReferencerType { get; set; }
    }

    /// <summary>
    /// Type of referencer (association), eg. a Data object that belongs to
    /// a product, or a key or user
    /// </summary>
    public enum DataObjectType : byte
    {
        User = 0,
        Product = 1,
        Key = 2
    }
}
