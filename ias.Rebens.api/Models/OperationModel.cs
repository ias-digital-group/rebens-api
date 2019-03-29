using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Operação
    /// </summary>
    public class OperationModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }
        /// <summary>
        /// Nome da empresa
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string CompanyName { get; set; }
        /// <summary>
        /// Documento da empresa
        /// </summary>
        [MaxLength(50)]
        public string CompanyDoc { get; set; }
        /// <summary>
        /// Logo
        /// </summary>
        [MaxLength(500)]
        public string Logo { get; set; }
        /// <summary>
        /// Domínio da opreação
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Domain { get; set; }
        /// <summary>
        /// Tipo da operação
        /// </summary>
        [Required]
        public int IdOperationType { get; set; }
        /// <summary>
        /// Tipo da operação
        /// </summary>
        public string OperationType { get { return Enums.EnumHelper.GetEnumDescription((Enums.OperationType)this.IdOperationType); } }
        /// <summary>
        /// Porcentagem do cashback
        /// </summary>
        public decimal? CachbackPercentage { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Contato
        /// </summary>
        public ContactModel Contact { get; set; }

        /// <summary>
        /// Lista de relacionamento de Operações com Contato
        /// </summary>
        public List<OperationContactModel> OperationContacts { get; set; }
        /// <summary>
        /// GUID gerado automáticamente
        /// </summary>
        public string Code { get; set; } 
        /// <summary>
        /// Construtor
        /// </summary>
        public OperationModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Operation e popula os atributos
        /// </summary>
        /// <param name="operation"></param>
        public OperationModel(Operation operation) {
            this.Id = operation.Id;
            this.Title = operation.Title;
            this.CompanyName = operation.CompanyName;
            this.CompanyDoc = operation.CompanyDoc;
            this.Logo = operation.Image;
            this.Domain = operation.Domain;
            this.IdOperationType = operation.IdOperationType;
            this.CachbackPercentage = operation.CashbackPercentage;
            this.Active = operation.Active;
            this.Code = operation.Code.ToString();

            if(operation.OperationContacts != null && operation.OperationContacts.Count >0)
            {
                OperationContacts = new List<OperationContactModel>();
                foreach (var item in operation.OperationContacts)
                    OperationContacts.Add(new OperationContactModel() { IdContact = item.IdContact, IdOperation = item.IdOperation });
            }
        }

        /// <summary>
        /// Retorna um objeto Operation com as informações
        /// </summary>
        /// <returns></returns>
        public Operation GetEntity()
        {
            return new Operation()
            {
                Id = this.Id,
                Title = this.Title,
                CompanyName = this.CompanyName,
                CompanyDoc = this.CompanyDoc,
                Image = this.Logo,
                Domain = this.Domain,
                IdOperationType = this.IdOperationType,
                CashbackPercentage = this.CachbackPercentage,
                Active = this.Active
            };
        }
    }
}
