using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Cliente do parceiro da Operação
    /// </summary>
    public class OperationPartnerCustomerModel
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
        /// Email
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }
        /// <summary>
        /// CPF
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Cpf { get; set; }
        /// <summary>
        /// Id da Operação
        /// </summary>
        [Required]
        public int IdOperationPartner { get; set; }
        /// <summary>
        /// Status (Novo = 1, Aprovado = 2, Reprovado = 3, Cadastrado = 4)
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// Nome do status
        /// </summary>
        public string StatusName { get; set; }

        public OperationPartnerCustomerModel(OperationPartnerCustomer customer)
        {
            this.Id = customer.Id;
            this.Name = customer.Name;
            this.Email = customer.Email;
            this.Cpf = customer.Cpf;
            this.IdOperationPartner = customer.IdOperationPartner;
            this.Status = customer.Status;
            this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.OperationPartnerCustomerStatus)customer.Status);
        }

        public OperationPartnerCustomer GetEntity()
        {
            return new OperationPartnerCustomer()
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Cpf = this.Cpf,
                IdOperationPartner = this.IdOperationPartner,
                Status = this.Status
            };
        }
    }
}
