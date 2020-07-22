using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class CompanyItem : Company
    {
        public Partner Partner { get; set; }
        public Operation Operation { get; set; }
        public string ItemName { get; set; }
        public string Logo { get; set; }

        public CompanyItem(Company company)
        {
            this.Id = company.Id;
            this.Name = company.Name;
            this.Cnpj = company.Cnpj;
            this.Type = company.Type;
            this.IdItem = company.IdItem;
            this.IdAddress = company.IdAddress;
            this.IdContact = company.IdContact;
            this.Active = company.Active;
            this.Address = company.Address;
            this.Contact = company.Contact;
            this.Created = company.Created;
            this.Modified = company.Modified;
        }
    }
}
