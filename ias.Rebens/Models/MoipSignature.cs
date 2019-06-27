using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class MoipSignature
    {
        public MoipSignature()
        {
            Payments = new HashSet<MoipPayment>();
            Invoices = new HashSet<MoipInvoice>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int IdCustomer { get; set; }
        public string PlanCode { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime NextInvoiceDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public int IdOperation { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<MoipInvoice> Invoices { get; set; }
        public virtual ICollection<MoipPayment> Payments { get; set; }
    }
}
