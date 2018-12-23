using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BenefitAddress
    {
        public int IdBenefit { get; set; }
        public int IdAddress { get; set; }

        public virtual Address IdAddressNavigation { get; set; }
        public virtual Benefit IdBenefitNavigation { get; set; }
    }
}
