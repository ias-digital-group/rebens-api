using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Contact
    {
        public Contact()
        {
            Partners = new HashSet<Partner>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public int? IdAddress { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Address IdAddressNavigation { get; set; }
        public virtual ICollection<Partner> Partners { get; set; }
    }
}
