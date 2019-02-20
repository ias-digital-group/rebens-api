using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CouponCampaign
    {
        public CouponCampaign()
        {
            Coupons = new HashSet<Coupon>();
        }

        public int Id { get; set; }
        public string CampaignId { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<Coupon> Coupons { get; set; }
    }
}
