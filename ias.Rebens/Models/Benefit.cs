using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Benefit
    {
        public Benefit()
        {
            Banners = new HashSet<Banner>();
            BenefitAddresses = new HashSet<BenefitAddress>();
            BenefitCategories = new HashSet<BenefitCategory>();
            BenefitOperations = new HashSet<BenefitOperation>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int IdAdminUser { get; set; }
        public string Image { get; set; }
        public DateTime? DueDate { get; set; }
        public string WebSite { get; set; }
        public decimal? MaxDiscountPercentageOnline { get; set; }
        public decimal? CpvpercentageOnline { get; set; }
        public decimal? MaxDiscountPercentageOffline { get; set; }
        public decimal? CpvpercentageOffline { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int IdBenefitType { get; set; }
        public bool Exclusive { get; set; }
        public bool Active { get; set; }
        public int IdIntegrationType { get; set; }
        public int IdPartner { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual BenefitType BenefitType { get; set; }
        public virtual IntegrationType IntegrationType { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
        public virtual ICollection<BenefitAddress> BenefitAddresses { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategories { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
    }
}
