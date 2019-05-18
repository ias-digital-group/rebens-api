using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Operation
    {
        public Operation()
        {
            AdminUsers = new HashSet<AdminUser>();
            BannerOperations = new HashSet<BannerOperation>();
            Benefits = new HashSet<Benefit>();
            BenefitOperations = new HashSet<BenefitOperation>();
            Customers = new HashSet<Customer>();
            Faqs = new HashSet<Faq>();
            FormContacts = new HashSet<FormContact>();
            FormEstablishments = new HashSet<FormEstablishment>();
            StaticTexts = new HashSet<StaticText>();
            OperationAddresses = new HashSet<OperationAddress>();
            OperationContacts = new HashSet<OperationContact>();
            OperationCustomers = new HashSet<OperationCustomer>();
            OperationPartners = new HashSet<OperationPartner>();
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
        public int? PublishStatus { get; set; }
        public int? IdLogError { get; set; }
        public Guid Code { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Deleted { get; set; }
        public string TemporarySubdomain { get; set; }
        public bool SubdomainCreated { get; set; }

        public int TemporaryPublishStatus { get; set; }

        public virtual LogError LogError { get; set; }
        public virtual ICollection<Benefit> Benefits { get; set; }
        public virtual ICollection<AdminUser> AdminUsers { get; set; }
        public virtual ICollection<BannerOperation> BannerOperations { get; set; }
        public virtual ICollection<BenefitOperation> BenefitOperations { get; set; }
        public virtual ICollection<Faq> Faqs { get; set; }
        public virtual ICollection<FormContact> FormContacts { get; set; }
        public virtual ICollection<FormEstablishment> FormEstablishments { get; set; }
        public virtual ICollection<StaticText> StaticTexts { get; set; }
        public virtual ICollection<OperationAddress> OperationAddresses { get; set; }
        public virtual ICollection<OperationContact> OperationContacts { get; set; }
        public virtual ICollection<OperationCustomer> OperationCustomers { get; set; }
        public virtual ICollection<OperationPartner> OperationPartners { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
