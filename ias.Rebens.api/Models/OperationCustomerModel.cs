using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Cliente Pré cadastrado
    /// </summary>
    public class OperationCustomerModel
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
        /// CPF
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CPF { get; set; }

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
        /// Email 1
        /// </summary>
        [MaxLength(500)]
        public string Email1 { get; set; }

        /// <summary>
        /// Email 2
        /// </summary>
        [MaxLength(500)]
        public string Email2 { get; set; }

        /// <summary>
        /// Se já se cadastrou
        /// </summary>
        public bool Signed { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationCustomerModel() { }

        /// <summary>
        /// Construtor que recebe um objeto IntegrationType, e popula os atributos
        /// </summary>
        /// <param name="type"></param>
        public OperationCustomerModel(OperationCustomer customer)
        {
            this.Id = customer.Id;
            this.Name = customer.Name;
            this.CPF = customer.CPF;
            this.Phone = customer.Phone;
            this.Cellphone = customer.Cellphone;
            this.Email1 = customer.Email1;
            this.Email2 = customer.Email2;
            this.Signed = customer.Signed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OperationCustomer GetEntity()
        {
            return new OperationCustomer()
            {
                Id = this.Id,
                Name = this.Name,
                CPF = this.CPF,
                Phone = this.Phone,
                Cellphone = this.Cellphone,
                Email1 = this.Email1,
                Email2 = this.Email2,
                Signed = this.Signed,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
