using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Parceiro com Endereço
    /// </summary>
    public class PartnerAddressModel
    {
        /// <summary>
        /// Id do parceiro
        /// </summary>
        [Required]
        public int IdPartner { get; set; }

        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
    }
}
