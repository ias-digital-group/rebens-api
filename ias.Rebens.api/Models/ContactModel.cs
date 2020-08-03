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
        /// Nome do contato
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Surname { get; set; }

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
        /// Telefone Comercial
        /// </summary>
        [MaxLength(50)]
        public string ComercialPhone { get; set; }

        /// <summary>
        /// Ramal do telefone comercial
        /// </summary>
        [MaxLength(50)]
        public string ComercialPhoneBranch { get; set; }

        /// <summary>
        /// Id do endereço
        /// </summary>
        public int? IdAddress { get; set; }

        /// <summary>
        /// Tipo de contato
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// Id do item vinculado a esse contato
        /// </summary>
        public int? IdItem { get; set; }

        /// <summary>
        /// Status 
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public AddressModel Address { get; set; }
        /// <summary>
        /// Nome do endereço
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RelationshipName { get; set; }
        public string TypeName { get; }

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
            if (contact != null)
            {
                this.Id = contact.Id;
                this.Name = contact.Name;
                this.Email = contact.Email;
                this.JobTitle = contact.JobTitle;
                this.Phone = contact.Phone;
                this.CellPhone = contact.CellPhone;
                this.IdAddress = contact.IdAddress;
                this.ComercialPhone = contact.ComercialPhone;
                this.ComercialPhoneBranch = contact.ComercialPhoneBranch;
                this.Surname = contact.Surname;
                this.Active = contact.Active;
                this.IdAddress = contact.IdAddress;
                this.IdItem = contact.IdItem;
                if (contact.Type.HasValue)
                {
                    this.Type = contact.Type;
                    this.TypeName = Enums.EnumHelper.GetEnumDescription((Enums.LogItem)contact.Type);
                }
                if (contact.IdAddress.HasValue && contact.Address != null)
                    this.Address = new AddressModel(contact.Address);
            }
        }

        /// <summary>
        /// Construtor que recebe um objeto Contact e popula os atributos
        /// </summary>
        /// <param name="contact"></param>
        public ContactModel(Entity.ContactListItem contact)
        {
            if (contact != null)
            {
                this.Id = contact.Id;
                this.Name = contact.Name;
                this.Email = contact.Email;
                this.JobTitle = contact.JobTitle;
                this.Phone = contact.Phone;
                this.CellPhone = contact.Cellphone;
                this.ComercialPhone = contact.ComercialPhone;
                this.Surname = contact.Surname;
                this.Active = contact.Active;
                this.IdItem = contact.IdItem;
                this.Type = contact.Type;
                this.AddressName = contact.Address;
                this.RelationshipName = contact.RelationshipName;
                this.ComercialPhoneBranch = contact.ComercialPhoneBranch;
                this.TypeName = Enums.EnumHelper.GetEnumDescription((Enums.LogItem)contact.Type);
            }
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
                IdAddress = this.IdAddress,
                ComercialPhone = this.ComercialPhone,
                ComercialPhoneBranch = this.ComercialPhoneBranch,
                Surname = this.Surname,
                Active = this.Active,
                Type = this.Type,
                IdItem = this.IdItem
            };
        }
    }
}
