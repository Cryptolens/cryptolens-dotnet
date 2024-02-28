using System;
using System.Collections.Generic;
using System.Globalization;

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
                return Name + "," + Email + "," + CompanyName + "," + Created.ToString(ConfigValues.DEFAULT_TIME_REPSENTATION, CultureInfo.InvariantCulture);
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            var customer = (Customer)obj;

            if (customer.CompanyName != CompanyName ||
                customer.Created != Created ||
                customer.Email != Email ||
                customer.Id != Id ||
                customer.Name != Name)
                return false;

            return true;

        }
    }
}
