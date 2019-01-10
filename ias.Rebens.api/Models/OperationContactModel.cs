using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento de Operação com Contato
    /// </summary>
    public class OperationContactModel
    {
        /// <summary>
        /// Id da Operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Id do Contato
        /// </summary>
        [Required]
        public int IdContact { get; set; }
    }
}
