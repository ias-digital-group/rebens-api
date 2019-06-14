using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Período de curso
    /// </summary>
    public class CoursePeriodModel
    {
        /// <summary>
        /// Id do período
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome do período
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Se o período está ativo ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CoursePeriodModel() { }

        /// <summary>
        /// Construtor que recebe um objeto CoursePeriod e popula os atributos
        /// </summary>
        /// <param name="coursePeriod"></param>
        public CoursePeriodModel(CoursePeriod coursePeriod)
        {
            this.Id = coursePeriod.Id;
            this.Name = coursePeriod.Name;
            this.Active = coursePeriod.Active;
            this.IdOperation = coursePeriod.IdOperation;
        }

        /// <summary>
        /// Retorna um objeto CoursePeriod com as informações
        /// </summary>
        /// <returns></returns>
        public CoursePeriod GetEntity()
        {
            return new CoursePeriod()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
