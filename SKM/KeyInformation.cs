using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKGL
{
    /// <summary>
    /// A class that stores information about a key. Note, if the Valid=false, no more information (creation date, etc) will be stored in the Key Information object.
    /// </summary>
    [Serializable]
    public class KeyInformation
    {
        /// <summary>
        /// True if the key is valid. False otherwise.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// The date when the key was generated.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The specified expiration date.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// The number of days a key should be valid.
        /// </summary>
        public int SetTime { get; set; }

        /// <summary>
        /// The number of days before the key expires.
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// The 8 different features that are stored in the key.
        /// </summary>
        public bool[] Features { get; set; }

        /// <summary>
        /// In some cases, KeyActivation will return the new key that will be stored in this variable. If there are no changes to the key, the current key will be stored here.
        /// </summary>
        public string NewKey { get; set; }

        /// <summary>
        /// The notes field of a given key. Make sure to enable access to notes field on http://serialkeymanager.com/Account/Manage.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The mid (machine id) that was provided as an input parameter. Mid will only be stored if 'signMid' was set to true.
        /// </summary>
        public string Mid { get; set; }

        /// <summary>
        /// When secure option is set to true, this variable will contain the signature of the information that the server returns. It is a signature of almost all variables stored in this variable except for NewKey.
        /// </summary>
        public string Signature { get; set; }
    }

}
