using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class OperationAddress
    {
        public int IdOperation { get; set; }
        public int IdAddress { get; set; }

        public virtual Address Addresses { get; set; }
        public virtual Operation Operations { get; set; }
    }
}
