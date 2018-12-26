using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BenefitOperation
    {
        public int IdBenefit { get; set; }
        public int IdOperation { get; set; }
        public int IdPosition { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Benefit Benefit { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual BenefitOperationPosition Position { get; set; }
    }
}
