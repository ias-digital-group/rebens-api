using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de resgate
    /// </summary>
    public class WithdrawModel
    {
        /// <summary>
        /// Id da conta para onde será transferido o dinheiro
        /// </summary>
        [Required]
        public int IdBankAccount { get; set; }
        /// <summary>
        /// Valor do resgate
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
    }
}
