using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model Customer Referal
    /// </summary>
    public class CustomerReferalModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id Cliente
        /// </summary>
        [Required]
        public int IdCustomer { get; set; }
        /// <summary>
        /// Nome do Cliente
        /// </summary>
        [MaxLength(200)]
        public string CustomerName { get; set; }
        /// <summary>
        /// Nome do indicado
        /// </summary>
        [MaxLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// Email do indicado
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Email { get; set; }
        /// <summary>
        /// id do Status
        /// </summary>
        public int IdStatus { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [MaxLength(100)]
        public string Status { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CustomerReferalModel() { }

        /// <summary>
        /// Construtor que popula 
        /// </summary>
        /// <param name="customerReferal"></param>
        public CustomerReferalModel(CustomerReferal customerReferal)
        {
            this.Id = customerReferal.Id;
            this.IdCustomer = customerReferal.IdCustomer;
            this.IdStatus = customerReferal.IdStatus;
            this.Name = customerReferal.Name;
            this.Email = customerReferal.Email;
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.CustomerReferalStatus)customerReferal.IdStatus);
            this.Date = customerReferal.Created;
            if (customerReferal.Customer != null)
                this.CustomerName = customerReferal.Customer.Name;
        }

        /// <summary>
        /// Retorna um objeto CustomerReferal
        /// </summary>
        /// <returns></returns>
        public CustomerReferal GetEntity()
        {
            return new CustomerReferal()
            {
                Id = this.Id,
                IdStatus = this.IdStatus,
                IdCustomer = this.IdCustomer,
                Name = this.Name,
                Email = this.Email,
                Created = this.Date,
                Modified = DateTime.UtcNow
            };
        }
    }
}
