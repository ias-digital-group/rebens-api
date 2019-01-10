using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Benefício com Operação
    /// </summary>
    public class BenefitOperationModel
    {
        /// <summary>
        /// Id do benefício
        /// </summary>
        [Required]
        public int IdBenefit { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
    }
}
