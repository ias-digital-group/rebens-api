using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Address
    {
        public Address()
        {
            BenefitAddresses = new HashSet<BenefitAddress>();
            Contacts = new HashSet<Contact>();
            PartnerAddresses = new HashSet<PartnerAddress>();
            OperationAddresses = new HashSet<OperationAddress>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<BenefitAddress> BenefitAddresses { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddresses { get; set; }
        public virtual ICollection<OperationAddress> OperationAddresses { get; set; }
    }
}
