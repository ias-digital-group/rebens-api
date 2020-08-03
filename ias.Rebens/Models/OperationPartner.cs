using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class OperationPartner
    {
        public OperationPartner()
        {
            Customers = new HashSet<Customer>();
            AdminUsers = new HashSet<AdminUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdOperation { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Doc { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<AdminUser> AdminUsers { get; set; }

    }
}
