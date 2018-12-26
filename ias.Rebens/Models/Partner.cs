using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Partner
    {
        public Partner()
        {
            Benefits = new HashSet<Benefit>();
            PartnerAddresses = new HashSet<PartnerAddress>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int IdContact { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual ICollection<Benefit> Benefits { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddresses { get; set; }
    }
}
