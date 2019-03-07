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
            BenefitUses = new HashSet<BenefitUse>();
            StaticTexts = new HashSet<StaticText>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int IdAdminUser { get; set; }
        public string Image { get; set; }
        public DateTime? DueDate { get; set; }
        public string Link { get; set; }
        public decimal? MaxDiscountPercentage { get; set; }
        public decimal? CPVPercentage { get; set; }
        public decimal? MinDiscountPercentage { get; set; }
        public decimal? CashbackAmount { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int IdBenefitType { get; set; }
        public bool Exclusive { get; set; }
        public bool Active { get; set; }
        public int IdIntegrationType { get; set; }
        public int IdPartner { get; set; }
        public string Call { get; set; }
        public string VoucherText { get; set; }
        public int? IdOperation { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
        public virtual ICollection<BenefitAddress> BenefitAddresses { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategories { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
        public virtual ICollection<BenefitUse> BenefitUses { get; set; }
        public virtual ICollection<StaticText> StaticTexts { get; set; }
    }
}
