using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Benefício com Endereço
    /// </summary>
    public class BenefitAddressModel
    {
        /// <summary>
        /// Id do benefício
        /// </summary>
        [Required]
        public int IdBenefit { get; set; }

        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
    }
}
