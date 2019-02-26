using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class MoipInvoice
    {
        public MoipInvoice()
        {
            Payments = new HashSet<MoipPayment>();
        }

        public int Id { get; set; }
        public int IdMoipSignature { get; set; }
        public decimal Amount { get; set; }
        public int Occurrence { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
       
        public virtual MoipSignature MoipSignature { get; set; }

        public virtual ICollection<MoipPayment> Payments { get; set; }
    }
}
