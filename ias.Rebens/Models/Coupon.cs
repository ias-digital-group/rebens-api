using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Coupon
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int IdCouponCampaign { get; set; }
        public string ValidationCode { get; set; }
        public string Campaign { get; set; }
        public string SingleUseCode { get; set; }
        public string SingleUseUrl { get; set; }
        public string WidgetValidationCode { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? PlayedDate { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string ClaimType { get; set; }
        public DateTime? ValidationDate { get; set; }
        public string ValidationValue { get; set; }
        public DateTime? VoidedDate { get; set; }
        public bool Locked { get; set; }
        public string Value { get; set; }
        public long SequenceId { get; set; }
        public int Status { get; set; }
        public DateTime VerifiedDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual CouponCampaign CouponCampaign { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
