using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CustomerSubscription
    {
        public int IdCustomer { get; set; }
        public int IdOperation { get; set; }
        public int IdSubscription { get; set; }
        public DateTime Created { get; set; }
    }
}
