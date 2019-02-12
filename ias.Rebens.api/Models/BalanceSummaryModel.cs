using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model da tela de Resgate
    /// </summary>
    public class BalanceSummaryModel
    {
        /// <summary>
        /// Saldo Bloqueado
        /// </summary>
        public decimal BlokedBalance { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// Saldo Disponível
        /// </summary>
        public decimal AvailableBalance { get; set; }
    }
}
