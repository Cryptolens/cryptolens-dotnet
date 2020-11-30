using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3
{
    public class Reseller
    {
        public int Id { get; set; }
        public int InviteId { get; set; }
        public int ResellerUserId { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
    }
}
