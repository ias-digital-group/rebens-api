using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class BenefitView
    {
        public int Id { get; set; }
        public int IdBenefit { get; set; }
        public int IdCustomer { get; set; }
        public DateTime Created { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Benefit Benefit { get; set; }
    }
}
