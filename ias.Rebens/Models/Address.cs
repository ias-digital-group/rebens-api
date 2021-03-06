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
            Companies = new HashSet<Company>();
            PartnerAddresses = new HashSet<PartnerAddress>();
            OperationAddresses = new HashSet<OperationAddress>();
            Customers = new HashSet<Customer>();
            CourseCollegeAddresses = new HashSet<CourseCollegeAddress>();
            CourseColleges = new HashSet<CourseCollege>();
            CourseAddresses = new HashSet<CourseAddress>();
            Partners = new HashSet<Partner>();
            Operations = new HashSet<Operation>();
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
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<CourseCollegeAddress> CourseCollegeAddresses { get; set; }
        public virtual ICollection<CourseCollege> CourseColleges { get; set; }
        public virtual ICollection<CourseAddress> CourseAddresses { get; set; }
        public virtual ICollection<OperationAddress> OperationAddresses { get; set; }
        public virtual ICollection<Partner> Partners { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<PartnerAddress> PartnerAddresses { get; set; }
    }
}
