using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cryptolens.SKM.Models
{
    public class CreateSessionResult : BasicResult
    {
        public string SessionId { get; set; }
    }
    public class CreateSessionModel
    {
        public int PaymentFormId { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }

        public string Heading { get; set; } // remember to set a max length.

        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string CustomField { get; set; }

        [MaxLength(1000)]
        public string Metadata { get; set; }

        public long Expires { get; set; }
    }
}
