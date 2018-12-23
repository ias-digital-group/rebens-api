using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Partner
    {
        public Partner()
        {
            Benefit = new HashSet<Benefit>();
            PartnerAddress = new HashSet<PartnerAddress>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int IdContact { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Contact IdContactNavigation { get; set; }
        public virtual ICollection<Benefit> Benefit { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddress { get; set; }
    }
}
