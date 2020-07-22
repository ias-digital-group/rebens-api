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
        /// Grau de parentesco
        /// (Pai = 1, Mãe = 2, Irmão = 3, Irmã = 4, Tio(a) = 5, Primo(a) = 6, Avô(ó) = 7)
        /// </summary>
        public int? DegreeOfKinship { get; set; }
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
        public CustomerReferalModel(Customer customer)
        {
            this.Id = customer.Id;
            this.IdCustomer = customer.IdCustomerReferer.Value;
            this.IdStatus = customer.ComplementaryStatus.Value;
            this.Name = customer.Name;
            this.Email = customer.Email;
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.CustomerComplementaryStatus)customer.ComplementaryStatus.Value);
            this.Date = customer.Created;
            this.DegreeOfKinship = customer.DegreeOfKinship;
            if (customer.CustomerReferer != null)
                this.CustomerName = customer.CustomerReferer.Name;
        }

        /// <summary>
        /// Retorna um objeto CustomerReferal
        /// </summary>
        /// <returns></returns>
        public Customer GetEntity()
        {
            return new Customer()
            {
                Id = this.Id,
                ComplementaryStatus = this.IdStatus,
                IdCustomerReferer = this.IdCustomer,
                Name = this.Name,
                Email = this.Email,
                Created = this.Date,
                DegreeOfKinship = this.DegreeOfKinship,
                Modified = DateTime.UtcNow
            };
        }
    }
}
