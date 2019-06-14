using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modalidade de curso
    /// </summary>
    public class CourseModalityModel
    {
        /// <summary>
        /// Id da modalidade
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome da modalidade
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Se a modalidade está ativa ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CourseModalityModel() { }

        /// <summary>
        /// Construtor que recebe um objeto CourseModality e popula os atributos
        /// </summary>
        /// <param name="coursePeriod"></param>
        public CourseModalityModel(CourseModality courseModality)
        {
            this.Id = courseModality.Id;
            this.Name = courseModality.Name;
            this.Active = courseModality.Active;
            this.IdOperation = courseModality.IdOperation;
        }

        /// <summary>
        /// Retorna um objeto CourseModality com as informações
        /// </summary>
        /// <returns></returns>
        public CourseModality GetEntity()
        {
            return new CourseModality()
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
