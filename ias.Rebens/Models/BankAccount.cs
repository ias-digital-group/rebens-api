using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class BankAccount
    {
        public int Id { get; set; }
        public int IdBank { get; set; }
        public int IdCustomer { get; set; }
        public string Type { get; set; }
        public string Branch { get; set; }
        public string AccountNumber { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
