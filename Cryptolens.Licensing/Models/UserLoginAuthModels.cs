using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;

namespace SKM.V3.Models
{
    public class CreateAuthRequestResult
    {
        //(byte[] authorizationToken, RSAParameters parameters)
        public byte[] AuthorizationToken { get; set; }
        public RSAParameters Parameters { get; set; }

    }

    public class GetLicenseKeysResult
    {
        public List<LicenseKey> Licenses { get; set; }
        public string Error { get; set; }
        public string LicenseKeyToken { get; set; }
    }

    internal class GetLicenseKeysModel
    {
        public bool Sign { get; set; }

        [DefaultValue("")]
        public string MachineCode { get; set; }

        public SignMethod SignMethod { get; set; }
    }

    internal class GetLicenseKeysResultStringSign : BasicResult
    {
        public string Results { get; set; }
        public string ActivatedMachineCodes { get; set; }
        public string Signature { get; set; }

    }

    internal class GetLicenseKeysResultLinqSign : BasicResult
    {
        public string Results { get; set; }
        public string ActivatedMachineCodes { get; set; }
        public string Signature { get; set; }
        public long SignDate { get; set; }

    }

    /// <summary>
    /// Sets what should be signed and returned when returning a license key object
    /// that is intended to be stored on the end user machine.
    /// </summary>
    public enum SignMethod
    {
        /// <summary>
        /// This one is the default for the .NET library, but is not very flexible.
        /// It should not be used in newer models and methods.
        /// </summary>
        LinqSign = 0, // default

        /// <summary>
        /// This one is the preferred way of signing any type of information since it
        /// does not care about the conversion, only about the actual string being signed.
        /// </summary>
        StringSign = 1 // treat the license key as separate json string wrapped inside basic result
    }


    internal class GetTokenModel : GetChallengeModel
    {
        public string SignedChallenge { get; set; }
        public long Date { get; set; }
    }

    internal class GetTokenResult : BasicResult
    {
        public string Token { get; set; }
    }


    public class GetChallengeModel
    {
        public string AuthorizationToken { get; set; }
    }

    internal class GetChallengeResult : BasicResult
    {
        public string Challenge { get; set; }
    }

    internal class AuthorizeAppModel
    {
        public string AuthorizationToken { get; set; }
        public string PublicKey { get; set; }
        public string Scope { get; set; }
        public string VendorAppName { get; set; }
        public int Expires { get; set; }
        public int TokenId { get; set; }
        public string MachineCode { get; set; }
        public string DeviceName { get; set; }

        public SignatureAlgorithm Algorithm { get; set; }
    }

    internal enum SignatureAlgorithm
    {
        RSA_2048 = 0
    }

    public class Scope
    {
        [DefaultValue(false)]
        public bool Deactivate { get; set; }

        [DefaultValue(false)]
        public bool CreateKey { get; set; }

        [DefaultValue(false)]
        public bool GetKeys { get; set; }

        [DefaultValue(false)]
        public bool Activate { get; set; }

        [DefaultValue(false)]
        public bool ExtendLicense { get; set; }

        [DefaultValue(false)]
        public bool AddFeature { get; set; }

        [DefaultValue(false)]
        public bool RemoveFeature { get; set; }

        [DefaultValue(false)]
        public bool AddDataObject { get; set; }

        [DefaultValue(false)]
        public bool ListDataObjects { get; set; }

        [DefaultValue(false)]
        public bool SetStringValue { get; set; }

        [DefaultValue(false)]
        public bool SetIntValue { get; set; }

        [DefaultValue(false)]
        public bool IncrementIntValue { get; set; }

        [DefaultValue(false)]
        public bool DecrementIntValue { get; set; }

        [DefaultValue(false)]
        public bool RemoveDataObject { get; set; }

        [DefaultValue(false)]
        public bool BlockKey { get; set; }

        [DefaultValue(false)]
        public bool UnblockKey { get; set; }

        [DefaultValue(false)]
        public bool KeyInfo { get; set; }

        [DefaultValue(false)]
        public bool GetFeatureDefinitions { get; set; }

        [DefaultValue(false)]
        public bool ChangeNotes { get; set; }

        [DefaultValue(false)]
        public bool GetCustomers { get; set; }

        [DefaultValue(false)]
        public bool ChangeCustomer { get; set; }

        [DefaultValue(false)]
        public bool AddCustomer { get; set; }
        [DefaultValue(false)]
        public bool RemoveCustomer { get; set; }
        [DefaultValue(false)]
        public bool GetCustomerLicenses { get; set; }

        [DefaultValue(false)]
        public bool TrialActivation { get; set; }

        [DefaultValue(false)]
        public bool MachineLockLimit { get; set; }

        [DefaultValue(false)]
        public bool GetLicenseKeys { get; set; }

        [DefaultValue(false)]
        public bool PaymentForms { get; set; }

        [DefaultValue(0)]
        public int LockToProduct { get; set; }

        [DefaultValue(0)]
        public int LockToKey { get; set; }


        [DefaultValue(0)]
        public int LockToFeature { get; set; }
    }
}
