using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de validação de cliente
    /// </summary>
    public class ValidateCustomerModel
    {
        /// <summary>
        /// Código do cliente
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
