using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
        /// Sobrenome
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Surname { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Sexo
        /// </summary>
        [MaxLength(1)]
        public string Gender { get; set; }
        /// <summary>
        /// Data de nascimento
        /// </summary>
        public string Birthday { get; set; }
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
        /// Avatar
        /// </summary>
        [MaxLength(500)]
        public string Picture { get; set; }
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
            this.Surname = customer.Surname;
            this.IdOperation = customer.IdOperation;
            this.Gender = customer.Gender.ToString();
            this.Birthday = customer.Birthday.HasValue ? customer.Birthday.Value.ToString("dd/MM/yyyy") : null;
            this.Email = customer.Email;
            this.IdAddress = customer.IdAddress;
            this.Cpf = customer.Cpf;
            this.RG = customer.RG;
            this.Picture = string.IsNullOrEmpty(customer.Picture) ? "https://res.cloudinary.com/rebens/image/upload/v1557186803/Portal/default-avatar.png" : customer.Picture;
            this.Phone = customer.Phone;
            this.Cellphone = customer.Cellphone;
            this.CustomerType = customer.CustomerType;
            this.Status = customer.Status;
            if(!string.IsNullOrEmpty(customer.Configuration))
                this.Configurations = Helper.Config.JsonHelper<List<Helper.Config.ConfigurationValue>>.GetObject(customer.Configuration);
            if (customer.IdAddress.HasValue && customer.Address != null)
                this.Address = new AddressModel(customer.Address);
        }

        /// <summary>
        /// Retorna um objeto Customer com as informações
        /// </summary>
        /// <returns></returns>
        public Customer GetEntity() {
            var ret = new Customer()
            {
                Id = this.Id,
                Name = this.Name,
                Surname = this.Surname,
                IdOperation = this.IdOperation,
                Gender = this.Gender != null ? this.Gender[0] : ' ',
                Email = this.Email,
                IdAddress = this.IdAddress,
                Cpf = this.Cpf,
                RG = this.RG,
                Phone = this.Phone,
                Cellphone = this.Cellphone,
                CustomerType = this.CustomerType,
                Status = this.Status,
                Picture = this.Picture,
                Configuration = this.Configurations != null && this.Configurations.Count > 0 ? null : Helper.Config.JsonHelper<List<Helper.Config.ConfigurationValue>>.GetString(this.Configurations),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            if(!string.IsNullOrEmpty(this.Birthday))
            {
                try
                {
                    var culture = new CultureInfo("pt-BR");
                    ret.Birthday = Convert.ToDateTime(this.Birthday, culture);
                }
                catch { }
            }
            return ret;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomerListItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        public int IdOperation { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Id do Status 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Nome do Status 
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// Nome da Operação
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CustomerListItem() { }

        /// <summary>
        /// Construtor que recebe um objeto Customer e já popula os atributos
        /// </summary>
        /// <param name="customer"></param>
        public CustomerListItem(CustomerReportItem customer)
        {
            this.Id = customer.Id;
            this.Name = customer.Name + " " + customer.Surname;
            this.IdOperation = customer.IdOperation;
            this.Email = customer.Email;
            this.Status = customer.Status;
            this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.CustomerStatus)customer.Status);
            this.OperationName = customer.OperationName;
        }
    }
}
