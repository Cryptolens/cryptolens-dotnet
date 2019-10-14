using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Models
{
    /// <summary>
    /// An assembly signature object consists both of the hash of the assembly
    /// and its location.
    /// </summary>
    public class AssemblySignature
    {
        /// <summary>
        /// The path of the assembly being signed
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The signature of the assembly.
        /// </summary>
        public string Signature { get; set; }

        public override string ToString()
        {
            return Path + ":" + Signature;
        }
    }
}
