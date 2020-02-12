using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CustomerPromoter
    {
        public int Id { get; set; }
        public int IdAminUser { get; set; }
        public int IdCustomer { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual AdminUser AdminUser { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
