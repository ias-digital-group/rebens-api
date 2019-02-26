﻿using System;
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
            CustomerReferals = new HashSet<CustomerReferal>();
            Coupons = new HashSet<Coupon>();
            Withdraws = new HashSet<Withdraw>();
            ZanoxSales = new HashSet<ZanoxSale>();
            Signatures = new HashSet<MoipSignature>();
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
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<BenefitUse> BenefitUses { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<CustomerReferal> CustomerReferals { get; set; }
        public virtual ICollection<MoipSignature> Signatures { get; set; }
        public virtual ICollection<Withdraw> Withdraws { get; set; }
        public virtual ICollection<ZanoxSale> ZanoxSales { get; set; }


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
