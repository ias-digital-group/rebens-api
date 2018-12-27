using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public int? IdAddress { get; set; }

        public AddressModel Address { get; set; }

        public ContactModel() { }

        public ContactModel(Contact contact)
        {
            this.Id = contact.Id;
            this.Name = contact.Name;
            this.Email = contact.Email;
            this.JobTitle = contact.JobTitle;
            this.Phone = contact.Phone;
            this.CellPhone = contact.CellPhone;
            this.IdAddress = contact.IdAddress;
            if (contact.IdAddress.HasValue && contact.Address != null)
                this.Address = new AddressModel(contact.Address);
        }

        public Contact GetEntity()
        {
            return new Contact()
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                JobTitle = this.JobTitle,
                Phone = this.Phone,
                CellPhone = this.CellPhone,
                IdAddress = this.IdAddress
            };
        }
    }
}
