using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Banner
    {
        public Banner()
        {
            BannerOperations = new HashSet<BannerOperation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public string Link { get; set; }
        public int Type { get; set; }
        public string BackgroundColor { get; set; }
        public int? IdBenefit { get; set; }
        public bool Active { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Benefit Benefit { get; set; }
        public virtual ICollection<BannerOperation> BannerOperations { get; set; }
    }
}
