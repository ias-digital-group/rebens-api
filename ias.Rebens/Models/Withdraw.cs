using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Withdraw
    {
        public int Id { get; set; }
        public int IdBankAccount { get; set; }
        public int IdCustomer { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual BankAccount BankAccount { get; set; }
    }
}
