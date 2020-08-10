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
        public int Status { get; set; }

        /// <summary>
        /// Nome do status
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Nome do parceiro
        /// </summary>
        public string OperationPartnerName { get; set; }
        
        /// <summary>
        /// Nome do Usuário que aprovou ou reprovou o cadastro
        /// </summary>
        public string AdminUserName { get; set; }


        /// <summary>
        /// Construtor
        /// </summary>
        public OperationPartnerCustomerModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="customer"></param>
        public OperationPartnerCustomerModel(Customer customer)
        {
            if(customer != null)
            this.Id = customer.Id;
            this.Name = customer.Name;
            this.Email = customer.Email;
            this.Cpf = customer.Cpf;
            this.IdOperationPartner = customer.IdOperationPartner.Value;
            this.Status = customer.ComplementaryStatus.Value;
            this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.CustomerComplementaryStatus)customer.Status);
            if (customer.OperationPartner != null)
                this.OperationPartnerName = customer.OperationPartner.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Customer GetEntity()
        {
            return new Customer()
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Cpf = this.Cpf,
                IdOperationPartner = this.IdOperationPartner,
                ComplementaryStatus = this.Status,
                CustomerType = (int)Enums.CustomerType.Partner,
                Active = true, 
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Status = (int)Enums.CustomerStatus.Validation
            };
        }
    }

    public class OperationPartnerCustomerListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public int IdOperationPartner { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string OperationName { get; set; }
        public string OperationPartnerName { get; set; }
        public string AdminUserName { get; set; }
        
        public OperationPartnerCustomerListItem() { }

        public OperationPartnerCustomerListItem(Customer customer)
        {
            if (customer != null)
            {
                this.Id = customer.Id;
                this.Name = customer.Name + " " + customer.Surname;
                this.Email = customer.Email;
                this.Cpf = customer.Cpf;
                this.Phone = customer.Phone;
                this.Cellphone = customer.Cellphone;
                this.IdOperationPartner = customer.IdOperationPartner.Value;
                this.Status = customer.ComplementaryStatus.Value;
                this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.CustomerComplementaryStatus)customer.Status);
                if (customer.OperationPartner != null)
                    this.OperationPartnerName = customer.OperationPartner.Name;
                if (customer.Operation != null)
                    this.OperationName = customer.Operation.Title;
            }
        }
    }
}
