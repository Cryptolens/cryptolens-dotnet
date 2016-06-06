using SKM.V3.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SKM.V3.Models
{
    public class LicenseKey
    {
        public int ProductId { get; set; }

        public int ID { get; set; }

        public string Key { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public int Period { get; set; }

        public bool F1 { get; set; }
        public bool F2 { get; set; }
        public bool F3 { get; set; }
        public bool F4 { get; set; }
        public bool F5 { get; set; }
        public bool F6 { get; set; }
        public bool F7 { get; set; }
        public bool F8 { get; set; }

        public string Notes { get; set; }

        public bool Block { get; set; }

        public long GlobalId { get; set; }

        public Customer Customer { get; set; }

        public List<ActivationData> ActivatedMachines { get; set; }

        public bool TrialActivation { get; set; }

        public bool AutomaticActivation { get; set; }

        public int MaxNoOfMachines { get; set; }

        public string AllowedMachines { get; set; }

        public List<DataObject> DataObjects { get; set; }

        public DateTime SignDate { get; set; }

        public string Signature { get; set; }


        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <returns></returns>
        public int DaysLeft()
        {
            return DaysLeft(false);

        }

        /// <summary>
        /// Returns the number of days left for a given license (time left). This method is particularly useful 
        /// when KeyInfo is not updated regularly, because TimeLeft will not be affected (stay constant).
        /// If your implementation checks the license with the server periodically, this method should be used instead of TimeLeft.
        /// </summary>
        /// <param name="zeroIfExpired">If true, when a license has expired, zero will be returned.</param>
        /// <returns></returns>
        public int DaysLeft(bool zeroIfExpired = false)
        {
            var days = Expires - DateTime.Today;

            if (zeroIfExpired)
                return days.Days < 0 ? 0 : days.Days;
            else
                return days.Days;

        }

        /// <summary>
        /// Creates a new <see cref="DataObject"/>.
        /// </summary>
        /// <param name="auth">Details such as Token and Version</param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <remarks>Note: for more details, please see 
        /// <a href="https://serialkeymanager.com/docs/api/v3/AddDataObject">https://serialkeymanager.com/docs/api/v3/AddDataObject</a> </remarks>
        /// <returns>Returns <see cref="DataObjectIdResult"/> or null.</returns>
        public DataObjectIdResult AddDataObject(string token, DataObject dataObject)
        {
            var parameters = new AddDataObjectModel
            {
                IntValue = dataObject.IntValue,
                StringValue = dataObject.StringValue,
                ReferencerType = DataObjectType.Key,
                ReferencerId = ID,
                Name = dataObject.Name
            };

            return HelperMethods.SendRequestToWebAPI3<DataObjectIdResult>(parameters, "/data/adddataobject/", token);
        }

    }
}
