using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BenefitCategory
    {
        public int IdBenefit { get; set; }
        public int IdCategory { get; set; }

        public virtual Benefit IdBenefitNavigation { get; set; }
        public virtual Category IdCategoryNavigation { get; set; }
    }
}
