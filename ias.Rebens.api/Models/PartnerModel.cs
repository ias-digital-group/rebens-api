using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Parceiro
    /// </summary>
    public class PartnerModel
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
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Logo do parceiro
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Logo { get; set; }
        /// <summary>
        /// Descrição do Parceiro
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Contatos do parceiro
        /// </summary>
        public List<ContactModel> Contacts { get; set; }

        /// <summary>
        /// Endereços do parceiro
        /// </summary>
        public List<AddressModel> Addresses { get; set; }


        /// <summary>
        /// Construtor
        /// </summary>
        public PartnerModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Partner e popula os atributos
        /// </summary>
        /// <param name="partner"></param>
        public PartnerModel(Partner partner) {
            this.Id = partner.Id;
            this.Name = partner.Name;
            this.Active = partner.Active;
            this.Logo = partner.Logo;
            if (partner.StaticText != null)
                this.Description = partner.StaticText.Html;
        }

        /// <summary>
        /// retorna um objeto Partner com as informações
        /// </summary>
        /// <returns></returns>
        public Partner GetEntity() {
            return new Partner()
            {
                Id = this.Id,
                Name = this.Name,
                Active = this.Active,
                Logo = this.Logo,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
