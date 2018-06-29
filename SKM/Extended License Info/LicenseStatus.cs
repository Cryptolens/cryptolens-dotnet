using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3
{
    public class LicenseStatus
    {
        // NB: all of this has to be signed, so use Martin's sign method for C++ in .NET

        public bool IsValid { get; set; }

        public InvalidityReason ReasonForInvalidity { get; set; }

        public bool Trial { get; set; }

        public bool TimeLimited { get; set; }

        public int TimeLeft { get; set; }

    }

    public enum InvalidityReason
    {
        None = 0,
        Expired = 1,
        Blocked = 2
    }
}
