using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Contact
    {
        public Contact()
        {
            PartnerContacts = new HashSet<PartnerContact>();
            OperationContacts = new HashSet<OperationContact>();
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

        public virtual Address Address { get; set; }
        public virtual ICollection<PartnerContact> PartnerContacts { get; set; }
        public virtual ICollection<OperationContact> OperationContacts { get; set; }
    }
}
