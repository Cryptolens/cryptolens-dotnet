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


        public override bool Equals(object obj)
        {
            LicenseStatus ls = obj as LicenseStatus;

            if (ls == null)
                return false;

            return ls.IsValid == IsValid &&
                   ls.ReasonForInvalidity == ReasonForInvalidity &&
                   ls.TimeLeft == TimeLeft &&
                   ls.TimeLimited == TimeLimited &&
                   ls.Trial == Trial;
        }
    }

    public enum InvalidityReason
    {
        None = 0,
        Expired = 1,
        Blocked = 2
    }
}
