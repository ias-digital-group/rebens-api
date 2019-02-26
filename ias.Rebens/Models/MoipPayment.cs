using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class MoipPayment
    {
        public int Id { get; set; }
        public int IdMoipInvoice { get; set; }
        public long MoipId { get; set; }
        public int IdMoipSignature { get; set; }
        public decimal Amount { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public int PaymentMethod { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string HolderName { get; set; }
        public string FirstSixDigits { get; set; }
        public string LastFourDigits { get; set; }
        public string Vault { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual MoipInvoice Invoice { get; set; }
        public virtual MoipSignature Signature { get; set; }
    }
}
