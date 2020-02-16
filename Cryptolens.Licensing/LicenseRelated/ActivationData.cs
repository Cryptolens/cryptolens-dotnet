using System;

namespace SKM.V3
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

        public string FriendlyName { get; set; }

        public override string ToString()
        {
            if (this != null)
                return Mid + "," + IP + "," + (Time.HasValue ? Time.Value.ToString(ConfigValues.DEFAULT_TIME_REPSENTATION) : "");
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            ActivationData data = (ActivationData)obj;

            if (data.IP != IP ||
                data.Mid != Mid ||
                data.Time != Time)
                return false;
            return true;
        }
    }
}
