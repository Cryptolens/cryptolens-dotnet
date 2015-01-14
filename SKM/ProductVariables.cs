using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKGL
{
    /// <summary>
    /// A class that stores product variables that are need to perform key validation/activation/generation through the API.
    /// </summary>
    public class ProductVariables
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// "uid" variable
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// "pid" variable
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// "hsum" variable
        /// </summary>
        public string HSUM { get; set; }
    }
}
