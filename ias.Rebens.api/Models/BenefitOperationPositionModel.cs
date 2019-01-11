using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Posição
    /// </summary>
    public class PositionModel
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
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public PositionModel() { }

        /// <summary>
        /// Construtor que recebe um objeto e já popula os atributos.
        /// </summary>
        /// <param name="benefitOperationPosition"></param>
        public PositionModel(BenefitOperationPosition benefitOperationPosition)
        {
            this.Id = benefitOperationPosition.Id;
            this.Name = benefitOperationPosition.Name;
            this.Active = benefitOperationPosition.Active;
        }
    }
}
