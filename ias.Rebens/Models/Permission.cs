using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Permission
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? IdParent { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Permission Parent { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }

    }
}
