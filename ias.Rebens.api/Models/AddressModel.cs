using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Endereço
    /// </summary>
    public class AddressModel
    {
        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome do endereço
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        [MaxLength(400)]
        public string Street { get; set; }

        /// <summary>
        /// Número
        /// </summary>
        [MaxLength(50)]
        public string Number { get; set; }

        /// <summary>
        /// Complemento (sala, apto, etc)
        /// </summary> 
        [MaxLength(50)]
        public string Complement { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        [MaxLength(200)]
        public string Neighborhood { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [MaxLength(200)]
        public string City { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        [MaxLength(200)]
        public string State { get; set; }

        /// <summary>
        /// País
        /// </summary>
        [MaxLength(200)]
        public string Country { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        [MaxLength(50)]
        public string Zipcode { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        [MaxLength(50)]
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [MaxLength(50)]
        public string Longitude { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public AddressModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Address e já popula os atributos
        /// </summary>
        /// <param name="addr"></param>
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

        /// <summary>
        /// Retorna um objeto Address com as informações
        /// </summary>
        /// <returns></returns>
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
