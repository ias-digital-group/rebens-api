using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento de Operação com Endereço
    /// </summary>
    public class OperationAddressModel
    {
        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
    }
}
