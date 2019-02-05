using System;
using System.ComponentModel.DataAnnotations;


namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Formulário de indicação de estabelecimento
    /// </summary>
    public partial class FormEstablishmentModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Email { get; set; }
        /// <summary>
        /// Nome estabelecimento
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Establishment { get; set; }
        /// <summary>
        /// Website estabelecimento
        /// </summary>
        [MaxLength(500)]
        public string WebSite { get; set; }
        /// <summary>
        /// Nome responsável
        /// </summary>
        [MaxLength(300)]
        public string Responsible { get; set; }
        /// <summary>
        /// Email responsável
        /// </summary>
        [MaxLength(500)]
        public string ResponsibleEmail { get; set; }
        /// <summary>
        /// Cidade
        /// </summary>
        [MaxLength(300)]
        public string City { get; set; }
        /// <summary>
        /// Estado
        /// </summary>
        [MaxLength(50)]
        public string State { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public FormEstablishmentModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="formEstablishment"></param>
        public FormEstablishmentModel(FormEstablishment formEstablishment)
        {
            this.Id = formEstablishment.Id;
            this.Name = formEstablishment.Name;
            this.Email = formEstablishment.Email;
            this.Establishment = formEstablishment.Establishment;
            this.WebSite = formEstablishment.WebSite;
            this.Responsible = formEstablishment.Responsible;
            this.ResponsibleEmail = formEstablishment.ResponsibleEmail;
            this.City = formEstablishment.City;
            this.State = formEstablishment.State;
        }

        public FormEstablishment GetEntity()
        {
            return new FormEstablishment() {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Establishment = this.Establishment,
                WebSite = this.WebSite,
                Responsible = this.Responsible,
                ResponsibleEmail = this.ResponsibleEmail,
                City = this.City,
                State = this.State,
                Created = DateTime.Now
            };
        }
    }
}
