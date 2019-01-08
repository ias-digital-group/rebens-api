using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class AddressModel
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(400)]
        public string Street { get; set; }
        [MaxLength(50)]
        public string Number { get; set; }
        [MaxLength(50)]
        public string Complement { get; set; }
        [MaxLength(200)]
        public string Neighborhood { get; set; }
        [MaxLength(200)]
        public string City { get; set; }
        [MaxLength(200)]
        public string State { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [MaxLength(50)]
        public string Zipcode { get; set; }
        [MaxLength(50)]
        public string Latitude { get; set; }
        [MaxLength(50)]
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
