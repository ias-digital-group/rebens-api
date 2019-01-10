using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Parceiro com Contato
    /// </summary>
    public class PartnerContactModel
    {
        /// <summary>
        /// Id do parceiro
        /// </summary>
        [Required]
        public int IdPartner { get; set; }

        /// <summary>
        /// Id do Contato
        /// </summary>
        [Required]
        public int IdContact { get; set; }
    }
}
