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
            FreeCourses = new HashSet<FreeCourse>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Cnpj { get; set; }
        public string CompanyName { get; set; }
        public int? IdStaticText { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Deleted { get; set; }
        public int Type { get; set; }
        public virtual StaticText StaticText { get; set; }
        public int? IdMainAddress { get; set; }
        public int? IdMainContact { get; set; }

        public virtual Address MainAddress { get; set; }
        public virtual Contact MainContact { get; set; }

        public virtual ICollection<Benefit> Benefits { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddresses { get; set; }
        public virtual ICollection<FreeCourse> FreeCourses { get; set; }
    }
}
