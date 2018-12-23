using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Operation
    {
        public Operation()
        {
            BannerOperation = new HashSet<BannerOperation>();
            BenefitOperation = new HashSet<BenefitOperation>();
            Faq = new HashSet<Faq>();
            StaticText = new HashSet<StaticText>();
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

        public virtual OperationType IdOperationTypeNavigation { get; set; }
        public virtual ICollection<BannerOperation> BannerOperation { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperation { get; set; }
        public virtual ICollection<Faq> Faq { get; set; }
        public virtual ICollection<StaticText> StaticText { get; set; }
    }
}
