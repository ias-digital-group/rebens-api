using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            WirecardPayments = new HashSet<WirecardPayment>();
        }

        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int IdOperation { get; set; }
        public string DispId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
        public string IP { get; set; }
        public string WirecardId { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }
        public DateTime? WirecardDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Operation Operation { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<WirecardPayment> WirecardPayments { get; set; }
    }
}
