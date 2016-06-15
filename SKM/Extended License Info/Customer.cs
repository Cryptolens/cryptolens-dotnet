using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SKM.V3.Internal
{
    public class Customer
    {
        public int Id { get; set; }

        //[MaxLength(100, ErrorMessage = "The name of the customer is limited to 100 characters.")]
        [StringValidator(MaxLength = 100)]
        public string Name { get; set; }

        //[MaxLength(100, ErrorMessage = "The name of the customer is limited to 100 characters.")]
        [StringValidator(MaxLength = 100)]
        public string Email { get; set; }

        //[MaxLength(100, ErrorMessage = "The name of the customer is limited to 100 characters.")]
        [StringValidator(MaxLength = 100)]
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
