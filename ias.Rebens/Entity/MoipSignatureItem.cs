using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class MoipSignatureItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int IdCustomer { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime NextInvoiceDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public int IdOperation { get; set; }
        public string OperationName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Customer Customer { get; set; }
    }
}
