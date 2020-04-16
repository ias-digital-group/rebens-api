using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class AdminUser
    {
        public AdminUser()
        {
            OperationPartnerCustomers = new HashSet<OperationPartnerCustomer>();
            Courses = new HashSet<Course>();
            FreeCourses = new HashSet<FreeCourse>();
            CustomerPromoters = new HashSet<CustomerPromoter>();
            LogActions = new HashSet<LogAction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }
        public string EncryptedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int? IdOperation { get; set; }
        public string Roles { get; set; }
        public bool Deleted { get; set; }
        public int? IdOperationPartner { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual OperationPartner OperationPartner { get; set; }
        public virtual ICollection<OperationPartnerCustomer> OperationPartnerCustomers { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<FreeCourse> FreeCourses { get; set; }
        public virtual ICollection<CustomerPromoter> CustomerPromoters { get; set; }
        public virtual ICollection<LogAction> LogActions { get; set; }

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
