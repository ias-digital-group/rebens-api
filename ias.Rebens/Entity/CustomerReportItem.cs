using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class CustomerReportItem : Customer
    {
        public string OperationName { get; set; }

        public CustomerReportItem() { }
        public CustomerReportItem(Customer customer, string operationName) {
            this.Id = customer.Id;
            this.Birthday = customer.Birthday;
            this.Cellphone = customer.Cellphone;
            this.Code = customer.Code;
            this.Cpf = customer.Cpf;
            this.Created = customer.Created;
            this.CustomerType = customer.CustomerType;
            this.Email = customer.Email;
            this.Gender = customer.Gender;
            this.IdAddress = customer.IdAddress;
            this.IdOperation = customer.IdOperation;
            this.Modified = customer.Modified;
            this.Name = customer.Name;
            this.OperationName = operationName;
            this.Phone = customer.Phone;
            this.RG = customer.RG;
            this.Status = customer.Status;
            this.Surname = customer.Surname;
        }

    }
}
