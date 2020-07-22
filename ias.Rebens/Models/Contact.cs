using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Contact
    {
        public Contact()
        {
            Companies = new HashSet<Company>();
            Partners = new HashSet<Partner>();
            Operations = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string ComercialPhone { get; set; }
        public string ComercialPhoneBranch { get; set; }
        public int? IdAddress { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int? Type { get; set; }
        public int? IdItem { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Partner> Partners { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
    }
}
