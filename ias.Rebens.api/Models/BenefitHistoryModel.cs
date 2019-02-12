using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model Hisórico de Benefício
    /// </summary>
    public class BenefitHistoryModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Id do benefício
        /// </summary>
        public int IdBenefit { get; set; }
        /// <summary>
        /// Nome do Benefício
        /// </summary>
        public string BenefitName { get; set; }
        /// <summary>
        /// Valor da compra
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Valor do retorno (cashback)
        /// </summary>
        public decimal Cashback { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string status { get; set; }
    }
}
