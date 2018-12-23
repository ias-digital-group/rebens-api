using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Address
    {
        public Address()
        {
            BenefitAddress = new HashSet<BenefitAddress>();
            Contact = new HashSet<Contact>();
            PartnerAddress = new HashSet<PartnerAddress>();
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

        public virtual ICollection<BenefitAddress> BenefitAddress { get; set; }
        public virtual ICollection<Contact> Contact { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddress { get; set; }
    }
}
