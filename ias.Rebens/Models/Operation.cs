﻿using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Operation
    {
        public Operation()
        {
            BannerOperations = new HashSet<BannerOperation>();
            BenefitOperations = new HashSet<BenefitOperation>();
            Faqs = new HashSet<Faq>();
            StaticTexts = new HashSet<StaticText>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDoc { get; set; }
        public string Image { get; set; }
        public string Domain { get; set; }
        public int? IdContact { get; set; }
        public int IdOperationType { get; set; }
        public decimal? CashbackPercentage { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual OperationType OperationType { get; set; }
        public virtual ICollection<BannerOperation> BannerOperations { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
        public virtual ICollection<Faq> Faqs { get; set; }
        public virtual ICollection<StaticText> StaticTexts { get; set; }
    }
}
