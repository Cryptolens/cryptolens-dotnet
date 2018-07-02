using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SKM.V3.Methods;

namespace SKM.V3
{
    /// <summary>
    /// This field is obtained when you either call <see cref="Key.Activate(string, Models.ActivateModel)"/>
    /// or <see cref="Key.GetKey(string, Models.KeyInfoModel)"/> with the Metadata parameter set to true.
    /// </summary>
    /// <remarks>Please note that all the data in this object is not signed by the server. It means that you should
    /// not cache this locally on the client machine. Also, please note that until there is support in the Web API to get
    /// a signed copy of this object, you should use this object on systems where you have full control (eg. not on end
    /// user systems).
    /// </remarks>
    public class LicenseStatus
    {
        /// <summary>
        /// Using 'feature definitions on the product page', this will be true if all conditions
        /// were satisfied.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// If <see cref="IsValid"/> is false, this field will contain the reason.
        /// </summary>
        public InvalidityReason ReasonForInvalidity { get; set; }

        /// <summary>
        /// Using 'feature definitions', this will be true if this license key is a trial license.
        /// </summary>
        public bool Trial { get; set; }

        /// <summary>
        /// Using 'feature definitions', this will be true if this license key is time limited.
        /// </summary>
        public bool TimeLimited { get; set; }

        /// <summary>
        /// The number of days left of a license key, if it's time-limited.
        /// </summary>
        public int TimeLeft { get; set; }

    }

    public enum InvalidityReason
    {
        None = 0,
        Expired = 1,
        Blocked = 2
    }
}
