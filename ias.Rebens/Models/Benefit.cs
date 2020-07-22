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
            BenefitViews = new HashSet<BenefitView>();
            StaticTexts = new HashSet<StaticText>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public DateTime? DueDate { get; set; }
        public string Link { get; set; }
        public decimal? MaxDiscountPercentage { get; set; }
        public decimal? CPVPercentage { get; set; }
        public decimal? MinDiscountPercentage { get; set; }
        public decimal? CashbackAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? AvailableCashback { get; set; }
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
        public int HomeHighlight { get; set; }
        public int HomeBenefitHighlight { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Deleted { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
        public virtual ICollection<BenefitAddress> BenefitAddresses { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategories { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
        public virtual ICollection<BenefitUse> BenefitUses { get; set; }
        public virtual ICollection<BenefitView> BenefitViews { get; set; }
        public virtual ICollection<StaticText> StaticTexts { get; set; }
    }
}
