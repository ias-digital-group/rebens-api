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
        /// Razão Social
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Cnpj
        /// </summary>
        public string Cnpj { get; set; }
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
        /// O tipo do parceiro (beneficios = 4, Cursos Livres = 19)
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Id do Endereço Principal
        /// </summary>
        public int? IdMainAddress { get; set; }
        /// <summary>
        /// Id do Contato Principal
        /// </summary>
        public int? IdMainContact { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }

        /// <summary>
        /// Contato Principal
        /// </summary>
        public ContactModel MainContact { get; set; }
        /// <summary>
        /// Endereço Principal
        /// </summary>
        public AddressModel MainAddress { get; set; }
        /// <summary>
        /// Contatos do parceiro
        /// </summary>
        public List<ContactModel> Contacts { get; }
        /// <summary>
        /// Endereços do parceiro
        /// </summary>
        public List<AddressModel> Addresses { get; }


        /// <summary>
        /// Construtor
        /// </summary>
        public PartnerModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Partner e popula os atributos
        /// </summary>
        /// <param name="partner"></param>
        public PartnerModel(Partner partner) {
            if (partner != null)
            {
                this.Id = partner.Id;
                this.Name = partner.Name;
                this.Active = partner.Active;
                this.Logo = partner.Logo;
                this.Type = partner.Type;
                this.IdMainAddress = partner.IdMainAddress;
                this.IdMainContact = partner.IdMainContact;
                this.CompanyName = partner.CompanyName;
                this.Cnpj = partner.Cnpj;
                if(partner.MainAddress != null && partner.MainAddress.Id > 0)
                    this.MainAddress = new AddressModel(partner.MainAddress);
                if(partner.MainContact != null && partner.MainContact.Id > 0)
                    this.MainContact = new ContactModel(partner.MainContact);
                
                if (partner.StaticText != null)
                    this.Description = partner.StaticText.Html;
            }
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
                Type = this.Type,
                CompanyName = this.CompanyName,
                Cnpj = this.Cnpj,
                IdMainAddress = this.IdMainAddress,
                IdMainContact = this.IdMainContact,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }

    public class PartnerListItemModel
    {
        /// <summary>
        /// Id da categoria
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Logo do Parceiro
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// Se está ativa ou inativa
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Data da criação
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// Nome do ususário que criou
        /// </summary>
        public string CreatedUserName { get; set; }
        /// <summary>
        /// Data da Modificação
        /// </summary>
        public string Modified { get; set; }
        /// <summary>
        /// Nome do último usuário que modificou
        /// </summary>
        public string ModifiedUserName { get; set; }
        /// <summary>
        /// O tipo de category (benefícios = 1, cursos livres = 2)
        /// </summary>
        public string Type { get; set; }

        public PartnerListItemModel() { }

        public PartnerListItemModel(Entity.PartnerListItem partner)
        {
            if (partner != null)
            {
                this.Id = partner.Id;
                this.Name = partner.Name;
                this.Logo = partner.Logo;
                this.Active = partner.Active;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(partner.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.CreatedUserName = partner.CreatedUserName;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(partner.Modified, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.ModifiedUserName = partner.ModifiedUserName;
                this.Type = Enums.EnumHelper.GetEnumDescription((Enums.LogItem)partner.IdType);
            }
        }
    }
}
