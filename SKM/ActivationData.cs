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
        /// The IP address of the client
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// The time of the request performed by the client
        /// </summary>
        public DateTime? Time { get; set; }

        public override string ToString()
        {
            if (this != null)
                return Mid + "," + IP + "," + Time;
            return base.ToString();
        }
    }
}
