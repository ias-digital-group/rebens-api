using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Customer
    {
        public Customer()
        {
            BankAccounts = new HashSet<BankAccount>();
            BenefitUses = new HashSet<BenefitUse>();
            BenefitViews = new HashSet<BenefitView>();
            CustomerReferals = new HashSet<Customer>();
            Coupons = new HashSet<Coupon>();
            Withdraws = new HashSet<Withdraw>();
            ZanoxSales = new HashSet<ZanoxSale>();
            Signatures = new HashSet<MoipSignature>();
            CourseViews = new HashSet<CourseView>();
            CourseUses = new HashSet<CourseUse>();
            CourseCustomerRates = new HashSet<CourseCustomerRate>();
            DrawItems = new HashSet<DrawItem>();
            Orders = new HashSet<Order>();
            Draws = new HashSet<ScratchcardDraw>();
            CustomerLogs = new HashSet<CustomerLog>();
            ZanoxProgramViews = new HashSet<ZanoxProgramView>();
            ZanoxIncentiveClicks = new HashSet<ZanoxIncentiveClick>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int IdOperation { get; set; }
        public char Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public int? IdAddress { get; set; }
        public string Cpf { get; set; }
        public string RG { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public int CustomerType { get; set; }
        public string EncryptedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public string Configuration { get; set; }
        public string Code { get; set; }
        public string Picture { get; set; }
        public int Status { get; set; }
        public string WirecardId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Active { get; set; }
        public int? SendinblueListId { get; set; }
        public int? IdOperationPartner { get; set; }
        public DateTime? SignUpDate { get; set; }
        public int? ComplementaryStatus { get; set; }
        public int? DegreeOfKinship { get; set; }
        public int? IdCustomerReferer { get; set; }
        public int? IdPromoter { get; set; }


        public virtual Operation Operation { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<BenefitUse> BenefitUses { get; set; }
        public virtual ICollection<BenefitView> BenefitViews { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual Customer CustomerReferer { get; set; }
        public virtual ICollection<MoipSignature> Signatures { get; set; }
        public virtual ICollection<Withdraw> Withdraws { get; set; }
        public virtual ICollection<ZanoxSale> ZanoxSales { get; set; }
        public virtual OperationPartner OperationPartner { get; set; }
        public virtual ICollection<CourseView> CourseViews { get; set; }
        public virtual ICollection<CourseUse> CourseUses { get; set; }
        public virtual ICollection<CourseCustomerRate> CourseCustomerRates { get; set; }
        public virtual ICollection<DrawItem> DrawItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual AdminUser Promoter { get; set; }
        public virtual ICollection<ScratchcardDraw> Draws { get; set; }
        public virtual ICollection<CustomerLog> CustomerLogs { get; set; }
        public virtual ICollection<Customer> CustomerReferals { get; set; }
        public virtual ICollection<ZanoxProgramView> ZanoxProgramViews { get; set; }
        public virtual ICollection<ZanoxIncentiveClick> ZanoxIncentiveClicks { get; set; }
        

        public void SetPassword(string password)
        {
            this.PasswordSalt = Helper.SecurityHelper.GenerateSalt();
            this.EncryptedPassword = Helper.SecurityHelper.EncryptPassword(password, this.PasswordSalt);
        }

        public bool CheckPassword(string password)
        {
            var encryptedPass = Helper.SecurityHelper.EncryptPassword(password, this.PasswordSalt);
            return encryptedPass == this.EncryptedPassword;
        }
    }
}
