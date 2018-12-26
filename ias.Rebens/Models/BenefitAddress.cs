using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BenefitAddress
    {
        public int IdBenefit { get; set; }
        public int IdAddress { get; set; }

        public virtual Address Address { get; set; }
        public virtual Benefit Benefit { get; set; }
    }
}
