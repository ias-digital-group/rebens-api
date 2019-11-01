using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class WirecardPayment
    {
        public string Id { get; set; }
        public int IdOrder { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string BilletLineCode { get; set; }
        public string BilletLink { get; set; }
        public string BilletLinkPrint { get; set; }
        public string CardBrand { get; set; }
        public string CardFirstSix { get; set; }
        public string CardLastFour { get; set; }
        public string CardHolderName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Order Order { get; set; }
    }
}
