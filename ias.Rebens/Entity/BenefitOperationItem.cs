using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class BenefitOperationItem
    {
        public int? IdBenefit { get; set; }
        public int IdOperation { get; set; }
        public int? IdPosition { get; set; }
        public string OperationName { get; set; }
    }
}
