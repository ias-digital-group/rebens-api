using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BenefitOperationPosition
    {
        public BenefitOperationPosition()
        {
            BenefitOperation = new HashSet<BenefitOperation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<BenefitOperation> BenefitOperation { get; set; }
    }
}
