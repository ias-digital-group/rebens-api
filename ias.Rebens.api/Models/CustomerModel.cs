using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Cliente
    /// </summary>
    public class CustomerModel
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
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Sexo
        /// </summary>
        [Required]
        [MaxLength(1)]
        public string Gender { get; set; }
        /// <summary>
        /// Data de nascimento
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }
        /// <summary>
        /// Id do Endereço
        /// </summary>
        public int? IdAddress { get; set; }
        /// <summary>
        /// CPF
        /// </summary>
        [MaxLength(50)]
        public string Cpf { get; set; }
        /// <summary>
        /// RG
        /// </summary>
        [MaxLength(50)]
        public string RG { get; set; }
        /// <summary>
        /// Telefone
        /// </summary>
        [MaxLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// Celular
        /// </summary>
        [MaxLength(50)]
        public string Cellphone { get; set; }
        /// <summary>
        /// Tipo de cliente
        /// </summary>
        [Required]
        public int CustomerType { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [Required]
        public int Status { get; set; }
        /// <summary>
        /// Lista de Configurações 
        /// </summary>
        public List<Helper.Config.ConfigurationValue> Configurations { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public AddressModel Address { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CustomerModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Customer e já popula os atributos
        /// </summary>
        /// <param name="customer"></param>
        public CustomerModel(Customer customer)
        {
            this.Id = customer.Id;
            this.Name = customer.Name;
            this.IdOperation = customer.IdOperation;
            this.Gender = customer.Gender.ToString();
            this.Birthday = customer.Birthday;
            this.Email = customer.Email;
            this.IdAddress = customer.IdAddress;
            this.Cpf = customer.Cpf;
            this.RG = customer.RG;
            this.Phone = customer.Phone;
            this.Cellphone = customer.Cellphone;
            this.CustomerType = customer.CustomerType;
            this.Status = customer.Status;
            if(!string.IsNullOrEmpty(customer.Configuration))
                this.Configurations = Helper.Config.ConfigurationHelper.GetConfigurationValues(customer.Configuration);
            if (customer.IdAddress.HasValue && customer.Address != null)
                this.Address = new AddressModel(customer.Address);
        }

        /// <summary>
        /// Retorna um objeto Customer com as informações
        /// </summary>
        /// <returns></returns>
        public Customer GetEntity() {
            return new Customer()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Gender = this.Gender[0],
                Birthday = this.Birthday,
                Email = this.Email,
                IdAddress = this.IdAddress,
                Cpf = this.Cpf,
                RG = this.RG,
                Phone = this.Phone,
                Cellphone = this.Cellphone,
                CustomerType = this.CustomerType,
                Status = this.Status,
                Configuration = this.Configurations != null && this.Configurations.Count > 0 ? null : Helper.Config.ConfigurationHelper.GetConfigurationValueString(this.Configurations),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
