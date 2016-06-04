using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKM.V3;

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
        /// NOTE: If you don't check the license regularly (i.e. at application start up),
        /// it's better to use <see cref="SKM.DaysLeft(KeyInformation)"/>. When <see cref="KeyInformation"/> is not
        /// updated, <see cref="TimeLeft"/> will not be updated either.
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// The 8 different features that are stored in the key.
        /// </summary>
        public bool[] Features { get; set; }


        /// <summary>
        /// The initial key used during activation or validation.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// In some cases, KeyActivation will return the new key that will be stored in this variable. If there are no changes to the key, this field will be left empty (in most cases). Changes to the key will only occur if you use SKGL instead of SKM15.
        /// If certain methods are used, this field will contain the initial key. However, it's better to use the Key field for that.
        /// </summary>
        public string NewKey { get; set; }

        /// <summary>
        /// The notes field of a given key. Make sure to enable access to notes field on http://serialkeymanager.com/Account/Manage.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The mid (machine id) that was provided as an input parameter. Mid will only be stored if 'signMid' was set to true. Null otherwise.
        /// </summary>
        public string Mid { get; set; }

        /// <summary>
        /// The product id (pid) that was provided as an input parameter. Pid will only be stored if 'signPid' was set to true. Null otherwise.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// The user id (uid) that was provided as an input parameter. Uid will only be stored if 'signUid' was set to true. Null otherwise.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The customer id associated with this key. Customer will only be stored if 'signCustomer' was set to true. Null otherwise.
        /// </summary>
        public int? Customer { get; set; }

        /// <summary>
        /// The date when the key was validated/activated. Date will only be stored if 'signDate' was set to true. Null otherwise.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// When secure option is set to true, this variable will contain the signature of the information that the server returns. It is a signature of almost all variables stored in this variable except for NewKey.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// This allows you to store a copy of a specific access token used to access information related to this key.
        /// </summary>
        public AuthDetails Auth { get; set; }

        /// <summary>
        /// This is the id of the license key.
        /// </summary>
        public long Id { get; set; }
    }

}
