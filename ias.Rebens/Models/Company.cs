using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public int Type { get; set; }
        public int IdItem { get; set; }
        public int IdAddress { get; set; }
        public int IdContact { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Address Address { get; set; }
    }
}
