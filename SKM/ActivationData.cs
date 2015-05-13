using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKGL
{
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
        /// 
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? Time { get; set; }
    }
}
