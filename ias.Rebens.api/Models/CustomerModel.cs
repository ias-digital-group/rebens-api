using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

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
        [MaxLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// Sobrenome
        /// </summary>
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
        /// Status de cadastro
        /// </summary>
        [Required]
        public int Status { get; set; }
        /// <summary>
        /// se o cliente está Ativo ou Inativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        public string Created { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Status > 0 ? Enums.EnumHelper.GetEnumDescription((Enums.CustomerStatus)this.Status) : ""; } }
        /// <summary>
        /// Lista de Configurações 
        /// </summary>
        public List<Helper.Config.ConfigurationValue> Configurations { get; set; }
        /// <summary>
        /// Endereço
        /// </summary>
        public AddressModel Address { get; set; }
        public string Fullname { get { return this.Name + " " + this.Surname; } }

        /// <summary>
        /// Retorna o nome do clube que esse usuário faz parte
        /// </summary>
        public string Operation { get; }
        /// <summary>
        /// Retorna o nome do parceiro que esse usuário faz parte
        /// </summary>
        public string OperationPartner { get; }
        public int? IdOperationPartner { get; set; }

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
            if (customer != null)
            {
                this.Id = customer.Id;
                this.Name = customer.Name;
                this.Surname = customer.Surname;
                this.IdOperation = customer.IdOperation;
                this.Gender = char.IsWhiteSpace(customer.Gender) || customer.Gender == '\0' ? "" : customer.Gender.ToString();
                this.Birthday = customer.Birthday.HasValue ? customer.Birthday.Value.ToString("dd/MM/yyyy") : null;
                //this.Birthday = customer.Birthday;
                this.Email = customer.Email;
                this.IdAddress = customer.IdAddress;
                this.Cpf = customer.Cpf;
                this.RG = customer.RG;
                this.Picture = string.IsNullOrEmpty(customer.Picture) ? "https://res.cloudinary.com/rebens/image/upload/v1557186803/Portal/default-avatar.png" : customer.Picture;
                if (string.IsNullOrEmpty(customer.Phone) || customer.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Length < 10)
                    this.Phone = "";
                else
                {
                    string tempPhone = customer.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                    this.Phone = $"{tempPhone.Substring(0, 2)} {tempPhone.Substring(2, 4)}-{tempPhone.Substring(6, 4)}";
                }
                if (string.IsNullOrEmpty(customer.Cellphone) || customer.Cellphone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Length < 11)
                    this.Cellphone = "";
                else
                {
                    string tempPhone = customer.Cellphone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                    this.Cellphone = $"{tempPhone.Substring(0, 2)} {tempPhone.Substring(2, 5)}-{tempPhone.Substring(7, 4)}";
                }

                this.CustomerType = customer.CustomerType;
                this.Status = customer.Status;
                this.Active = customer.Active;
                var culture = new CultureInfo("pt-BR");
                this.Created = Convert.ToDateTime(customer.Created, culture).ToLocalTime().ToString("dd/MM/yyyy HH:mm", culture);
                if (!string.IsNullOrEmpty(customer.Configuration))
                    this.Configurations = Helper.Config.JsonHelper<List<Helper.Config.ConfigurationValue>>.GetObject(customer.Configuration);
                if (customer.IdAddress.HasValue && customer.Address != null)
                    this.Address = new AddressModel(customer.Address);
                else
                    this.Address = new AddressModel();
                if (customer.Operation != null)
                    this.Operation = customer.Operation.Title;
                if (customer.OperationPartner != null)
                {
                    this.OperationPartner = customer.OperationPartner.Name;
                    this.IdOperationPartner = customer.IdOperationPartner;
                }
            }
        }

        /// <summary>
        /// Retorna um objeto Customer com as informações
        /// </summary>
        /// <returns></returns>
        public Customer GetEntity() {
            DateTime? birthDt = null;
            if (this.Birthday != null && this.Birthday.Length == 10)
            {
                try
                {
                    birthDt = new DateTime(int.Parse(this.Birthday.Split('/')[2]), int.Parse(this.Birthday.Split('/')[1]), int.Parse(this.Birthday.Split('/')[0]));
                }
                catch { }
            }
            var ret = new Customer()
            {
                Id = this.Id,
                Name = this.Name,
                Surname = this.Surname,
                IdOperation = this.IdOperation,
                Gender = this.Gender != null && this.Gender != "" ? this.Gender.ToUpper()[0] : ' ',
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
                Modified = DateTime.UtcNow,
                Birthday = birthDt,
                Active = this.Active
            };

            return ret;
        }

        public Address GetAddress()
        {
            if(this.Address != null)
            {
                return new Address()
                {
                    Id = this.Address.Id,
                    City = this.Address.City,
                    Complement = this.Address.Complement,
                    Country = this.Address.Country,
                    Latitude = this.Address.Latitude,
                    Longitude = this.Address.Longitude,
                    Modified = DateTime.UtcNow,
                    Created = DateTime.UtcNow,
                    Name = this.Address.Name,
                    Neighborhood = this.Address.Neighborhood,
                    Number = this.Address.Number,
                    State = this.Address.State,
                    Street = this.Address.Street,
                    Zipcode = this.Address.Zipcode
                };
            }
            return null;
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
