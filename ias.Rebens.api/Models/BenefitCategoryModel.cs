using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Beneficio com Categoria
    /// </summary>
    public class BenefitCategoryModel
    {
        /// <summary>
        /// Id do benefício
        /// </summary>
        [Required]
        public int IdBenefit { get; set; }

        /// <summary>
        /// Id da categoria
        /// </summary>
        [Required]
        public int IdCategory { get; set; }
    }
}
