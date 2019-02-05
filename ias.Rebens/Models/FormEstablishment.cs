using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class FormEstablishment
    {
        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Establishment { get; set; }
        public string WebSite { get; set; }
        public string Responsible { get; set; }
        public string ResponsibleEmail { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime Created { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
