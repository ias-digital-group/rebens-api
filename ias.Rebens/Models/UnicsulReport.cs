using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Models
{
    public class UnicsulReport
    {
        public UnicsulReportSummary Summary { get; set; }
        public List<UnicsulReportBenefit> BenefitViews { get; set; }
        public List<UnicsulReportBenefit> BenefitsUsed { get; set; }
        public List<UnicsulReportCustomer> Customers { get; set; }
    }

    public class UnicsulReportCustomer
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Birthday { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Created { get; set; }
        public string LastLogin { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }

        public UnicsulReportCustomer(Customer customer)
        {
            if (customer != null)
            {
                this.Name = customer.Name;
                this.Surname = customer.Surname;
                this.Birthday = customer.Birthday.HasValue ? customer.Birthday.Value.ToString("dd/MM/yyyy") : "";
                this.RG = customer.RG;
                this.CPF = customer.Cpf;
                this.Phone = customer.Phone;
                this.Cellphone = customer.Cellphone;
                this.Email = customer.Email;
                if (customer.Address != null)
                {
                    this.Street = customer.Address.Street;
                    this.Number = customer.Address.Number;
                    this.Complement = customer.Address.Complement;
                    this.Zipcode = customer.Address.Zipcode;
                    this.City = customer.Address.City;
                    this.State = customer.Address.State;
                }
                this.StatusId = customer.Status;
                this.Status = Enums.EnumHelper.GetEnumDescription((Enums.CustomerStatus)customer.Status);
                this.Created = customer.Created.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
    }

    public class UnicsulReportBenefit
    {
        public string PartnerName { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public int TotalWeek { get; set; }
    }

    public class UnicsulReportSummary
    {
        public int Complete { get; set; }
        public int Validate { get; set; }
        public int Incomplete { get; set; }
        public int PreSign { get; set; }

        public int CompleteWeek { get; set; }
        public int ValidateWeek { get; set; }
        public int IncompleteWeek { get; set; }
    }
}
