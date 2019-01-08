using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class OperationContact
    {
        public int IdOperation { get; set; }
        public int IdContact { get; set; }

        public virtual Contact Contacts { get; set; }
        public virtual Operation Operations { get; set; }
    }
}
