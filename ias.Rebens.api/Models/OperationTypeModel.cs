using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Tipo de Operação
    /// </summary>
    public class OperationTypeModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationTypeModel() { }

        /// <summary>
        /// Construtor que recebe um objeto OperationType e popula os atributos
        /// </summary>
        /// <param name="type"></param>
        public OperationTypeModel(OperationType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
