﻿using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Benefit
    {
        public Benefit()
        {
            Banner = new HashSet<Banner>();
            BenefitAddress = new HashSet<BenefitAddress>();
            BenefitCategory = new HashSet<BenefitCategory>();
            BenefitOperation = new HashSet<BenefitOperation>();
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

        public virtual BenefitType IdBenefitTypeNavigation { get; set; }
        public virtual IntegrationType IdIntegrationTypeNavigation { get; set; }
        public virtual Partner IdPartnerNavigation { get; set; }
        public virtual ICollection<Banner> Banner { get; set; }
        public virtual ICollection<BenefitAddress> BenefitAddress { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategory { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperation { get; set; }
    }
}
