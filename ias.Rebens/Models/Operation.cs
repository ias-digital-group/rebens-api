using System;
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
            OperationAddresses = new HashSet<OperationAddress>();
            OperationContacts = new HashSet<OperationContact>();
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDoc { get; set; }
        public string Image { get; set; }
        public string Domain { get; set; }
        public int IdOperationType { get; set; }
        public decimal? CashbackPercentage { get; set; }
        public bool Active { get; set; }
        public Guid Code { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual OperationType OperationType { get; set; }
        public virtual ICollection<BannerOperation> BannerOperations { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
        public virtual ICollection<Faq> Faqs { get; set; }
        public virtual ICollection<StaticText> StaticTexts { get; set; }
        public virtual ICollection<OperationAddress> OperationAddresses { get; set; }
        public virtual ICollection<OperationContact> OperationContacts { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
