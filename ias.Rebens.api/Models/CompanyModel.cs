using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class CompanyModel
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
        /// Cnpj
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Cnpj { get; set; }
        /// <summary>
        /// Tipo (1 = Operação | 2 = Parceiro)
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Id da operação ou parceiro vinculado a essa empresa
        /// </summary>
        [Required]
        public int IdItem { get; set; }
        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
        /// <summary>
        /// Id do contato
        /// </summary>
        [Required]
        public int IdContact { get; set; }
        /// <summary>
        /// Se a empresa está ativa ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Contato
        /// </summary>
        public ContactModel Contact { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public AddressModel Address { get; set; }
        /// <summary>
        /// Operação caso seja uma empresa do tipo operação
        /// </summary>
        public OperationModel Operation { get; set; }
        /// <summary>
        /// Parceiro caso seja uma empresa do tipo parceiro
        /// </summary>
        public PartnerModel Partner { get; set; }
        /// <summary>
        /// Nome da Operação ou parceiro vinculado a essa empresa
        /// </summary>
        public string ItemName { get; }
        public string Logo { get; }

        public CompanyModel() { }
        public CompanyModel(Company company) {
            if (company != null)
            {
                this.Id = company.Id;
                this.Name = company.Name;
                this.Cnpj = company.Cnpj;
                this.Type = company.Type;
                this.IdItem = company.IdItem;
                this.IdAddress = company.IdAddress;
                this.IdContact = company.IdContact;
                this.Active = company.Active;

                if (company.Address != null)
                    this.Address = new AddressModel(company.Address);
                if (company.Contact != null)
                    this.Contact = new ContactModel(company.Contact);
            }
        }

        public CompanyModel(CompanyItem company)
        {
            if (company != null)
            {
                this.Id = company.Id;
                this.Name = company.Name;
                this.Cnpj = company.Cnpj;
                this.Type = company.Type;
                this.IdItem = company.IdItem;
                this.IdAddress = company.IdAddress;
                this.IdContact = company.IdContact;
                this.Active = company.Active;
                this.ItemName = company.ItemName;
                this.Logo = company.Logo;

                if (this.Type == (int)Enums.LogItem.Operation && company.Operation != null)
                    this.Operation = new OperationModel(company.Operation);
                if (this.Type == (int)Enums.LogItem.Partner && company.Partner != null)
                    this.Partner = new PartnerModel(company.Partner);
                if (company.Address != null)
                    this.Address = new AddressModel(company.Address);
                if (company.Contact != null)
                    this.Contact = new ContactModel(company.Contact);
            }
        }

        public Company GetEntity()
        {
            return new Company()
            {
                Id = this.Id,
                Name = this.Name,
                Active = this.Active,
                Type = this.Type,
                IdAddress = this.IdAddress,
                Cnpj = this.Cnpj,
                IdContact = this.IdContact,
                IdItem = this.IdItem,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
