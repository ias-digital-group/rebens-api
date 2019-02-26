using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class BenefitUse
    {
        public int Id { get; set; }
        public int IdBenefit { get; set; }
        public int IdCustomer { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int IdBenefitType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Comission { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Benefit Benefit { get; set; }
        public virtual BenefitType BenefitType { get; set; }
    }
}
