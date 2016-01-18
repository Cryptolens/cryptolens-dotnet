
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
