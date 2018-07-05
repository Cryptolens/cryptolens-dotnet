using System;
using System.Collections.Generic;

namespace SKM.V3
{
    /// <summary>
    /// Information about a customer. Each license key may be assigned a customer.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public DateTime Created { get; set; }
        public override string ToString()
        {
            if (this != null)
                return Name + "," + Email + "," + CompanyName + "," + Created.ToString(ConfigValues.DEFAULT_TIME_REPSENTATION);
            return base.ToString();
        }
    }
}
