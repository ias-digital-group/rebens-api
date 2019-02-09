using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Bank
    {
        public Bank()
        {
            BankAccounts = new HashSet<BankAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Initials { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}
