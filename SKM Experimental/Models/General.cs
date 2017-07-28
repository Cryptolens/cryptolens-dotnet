using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cryptolens.SKM.Models
{
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

    [Serializable]
    public class LicenseKey
    {
        public int ProductId { get; set; }

        public int ID { get; set; }

        public string Key { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public int Period { get; set; }

        public bool F1 { get; set; }
        public bool F2 { get; set; }
        public bool F3 { get; set; }
        public bool F4 { get; set; }
        public bool F5 { get; set; }
        public bool F6 { get; set; }
        public bool F7 { get; set; }
        public bool F8 { get; set; }

        public string Notes { get; set; }

        public bool Block { get; set; }

        public long GlobalId { get; set; }

        public Customer Customer { get; set; }

        public List<ActivationData> ActivatedMachines { get; set; }

        public bool TrialActivation { get; set; }

        public int MaxNoOfMachines { get; set; }

        public string AllowedMachines { get; set; }

        public List<DataObject> DataObjects { get; set; }

        public DateTime SignDate { get; set; }

        public string Signature { get; set; }

    }

    /// <summary>
    /// Information about a customer. Each license key may be assigned a customer.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public DateTime Created { get; set; }

    }

    /// <summary>
    /// This is the structure of each entry that will be returned by GetActivatedMachines.
    /// </summary>
    public class ActivationData
    {
        /// <summary>
        /// The machine code
        /// </summary>
        public string Mid { get; set; }

        /// <summary>
        /// The IP address of the client
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// The time of the request performed by the client
        /// </summary>
        public DateTime? Time { get; set; }

    }

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


        private string stringValue;
        /// <summary>
        /// A string value (text) to store. Max 100 characters.
        /// </summary>
        /// <remarks>Do not assign any values to this property.
        /// Instead, please use <see cref="SetStringValue(string, string)"/></remarks>
        public string StringValue { get; set; }

        /// <summary>
        /// An int value (int32) to store.
        /// </summary>
        /// <remarks>Do not assign any values to this property.
        /// Instead, please use <see cref="SetIntValue(string, int)"/></remarks>
        public int IntValue { get; set; }
    }

    public class KeyInfoResult : BasicResult
    {
        public LicenseKey LicenseKey { get; set; }
    }

}
