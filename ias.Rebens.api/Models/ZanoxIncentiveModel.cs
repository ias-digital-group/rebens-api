using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ZanoxIncentiveModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do programa
        /// </summary>
        public int IdProgram { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tipo
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Data da criação na Zanox
        /// </summary>
        public DateTime? ZanoxCreated { get; set; }
        /// <summary>
        /// Data da última modificação na Zanox
        /// </summary>
        public DateTime? ZanoxModified { get; set; }
        /// <summary>
        /// Inicio de validade 
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Fim da validade
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// Informação do publicador
        /// </summary>
        public string PublisherInfo { get; set; }
        /// <summary>
        /// Informação para o cliente
        /// </summary>
        public string CustomerInfo { get; set; }
        /// <summary>
        /// Restrição de uso do incentivo
        /// </summary>
        public string Restriction { get; set; }
        /// <summary>
        /// Código
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Moeda
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Data criação
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Data última modificação
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// Status (ativo = true, inativo = false)
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        public ZanoxIncentiveModel() { }
        public ZanoxIncentiveModel(ZanoxIncentive incentive) {
            if(incentive != null)
            {
                this.Id = incentive.Id;
                this.Name = incentive.Name;
                this.IdProgram = incentive.IdProgram;
                this.Active = incentive.Active;
                this.Amount = incentive.Amount;
                this.Code = incentive.Code;
                this.Created = incentive.Created;
                this.Currency = incentive.Currency;
                this.CustomerInfo = incentive.CustomerInfo;
                this.End = incentive.End;
                this.Modified = incentive.Modified;
                this.PublisherInfo = incentive.PublisherInfo;
                this.Restriction = incentive.Restriction;
                this.Start = incentive.Start;
                this.Type = incentive.Type;
                this.ZanoxCreated = incentive.ZanoxCreated;
                this.ZanoxModified = incentive.ZanoxModified;
                this.Url = incentive.Url;
            }
        }

    }
}
