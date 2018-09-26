using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3
{
    public class MessageObject
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public long Created { get; set; }
        public string Channel { get; set; }
    }
}
