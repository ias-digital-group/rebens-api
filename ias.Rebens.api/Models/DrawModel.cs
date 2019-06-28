using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Sorteio
    /// </summary>
    public class DrawModel
    {
        /// <summary>
        /// Id do sorteio
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome do sorteio
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Data inicial da vigência
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Data final da vigência
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Quantidade de números da sorte
        /// </summary>
        [Required]
        public int Quantity { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// se o sorteio está ativo
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// se o já foram gerados os números da sorte
        /// </summary>
        public bool Generated { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public DrawModel() { }
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="draw"></param>
        public DrawModel(Draw draw) {
            this.Id = draw.Id;
            this.Name = draw.Name;
            this.StartDate = draw.StartDate;
            this.EndDate = draw.EndDate;
            this.Quantity = draw.Quantity;
            this.IdOperation = draw.IdOperation;
            this.Active = draw.Active;
            this.Generated = draw.Generated;
        }

        /// <summary>
        /// Retorna um objeto Draw com as informações
        /// </summary>
        /// <returns>Draw</returns>
        public Draw GetEntity()
        {
            return new Draw()
            {
                Id = this.Id,
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Quantity = this.Quantity,
                IdOperation = this.IdOperation,
                Active = this.Active
            };
        }
    }
}
