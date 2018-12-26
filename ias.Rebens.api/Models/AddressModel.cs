using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class AddressModel
    {
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

        public AddressModel() { }

        public AddressModel(Address addr)
        {
            this.Id = addr.Id;
            this.Name = addr.Name;
            this.Street = addr.Street;
            this.Number = addr.Number;
            this.Complement = addr.Complement;
            this.Neighborhood = addr.Neighborhood;
            this.City = addr.City;
            this.State = addr.State;
            this.Country = addr.Country;
            this.Zipcode = addr.Zipcode;
            this.Latitude = addr.Latitude;
            this.Longitude = addr.Longitude;
        }

        public Address GetEntity()
        {
            return new Address()
            {
                Id = this.Id,
                Name = this.Name,
                Street = this.Street,
                Number = this.Number,
                Complement = this.Complement,
                Neighborhood = this.Neighborhood,
                City = this.City,
                State = this.State,
                Country = this.Country,
                Zipcode = this.Zipcode,
                Latitude = this.Latitude,
                Longitude = this.Longitude
            };
        }
    }
}
