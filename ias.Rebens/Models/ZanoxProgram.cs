using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class ZanoxProgram
    {
        public ZanoxProgram()
        {
            this.Incentives = new HashSet<ZanoxIncentive>();
            ZanoxProgramViews = new HashSet<ZanoxProgramView>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? AdRank { get; set; }
        public string Description { get; set; }
        public string LocalDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Terms { get; set; }
        public decimal? MaxCommissionPercent { get; set; }
        public decimal? MinCommissionPercent { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Active { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedDate { get; set; }


        public virtual ICollection<ZanoxIncentive> Incentives { get; set; }
        public virtual ICollection<ZanoxProgramView> ZanoxProgramViews { get; set; }
    }
}
