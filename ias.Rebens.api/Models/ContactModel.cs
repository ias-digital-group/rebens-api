using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Contato
    /// </summary>
    public class ContactModel
    {
        /// <summary>
        /// Id do contato
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome do contato
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [MaxLength(400)]
        public string Email { get; set; }

        /// <summary>
        /// Profissão
        /// </summary>
        [MaxLength(200)]
        public string JobTitle { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        [MaxLength(50)]
        public string Phone { get; set; }

        /// <summary>
        /// Celular
        /// </summary>
        [MaxLength(50)]
        public string CellPhone { get; set; }

        /// <summary>
        /// Id do endereço
        /// </summary>
        public int? IdAddress { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public AddressModel Address { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public ContactModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Contact e popula os atributos
        /// </summary>
        /// <param name="contact"></param>
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

        /// <summary>
        /// Retorna um objeto Contact com as informações
        /// </summary>
        /// <returns></returns>
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
