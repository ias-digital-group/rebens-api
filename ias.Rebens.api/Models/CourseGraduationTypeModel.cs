using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modalidade de curso
    /// </summary>
    public class CourseGraduationTypeModel
    {
        /// <summary>
        /// Id do tipo de graduação
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome do tipo de graduação
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Se o tipo de graduação está ativa ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Id do Pai (1=graduação, 2=Pós Graduação)
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Nome do Pai
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CourseGraduationTypeModel() { }

        /// <summary>
        /// Construtor que recebe um objeto CourseGraduationType e popula os atributos
        /// </summary>
        /// <param name="courseGraduationType"></param>
        public CourseGraduationTypeModel(CourseGraduationType courseGraduationType)
        {
            this.Id = courseGraduationType.Id;
            this.Name = courseGraduationType.Name;
            this.Active = courseGraduationType.Active;
            this.IdOperation = courseGraduationType.IdOperation;
            this.ParentId = courseGraduationType.Parent;
            this.ParentName = Enums.EnumHelper.GetEnumDescription((Enums.GraduationTypeParent)this.ParentId);
        }

        /// <summary>
        /// Retorna um objeto CourseGraduationType com as informações
        /// </summary>
        /// <returns></returns>
        public CourseGraduationType GetEntity()
        {
            return new CourseGraduationType()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Parent = this.ParentId
            };
        }
    }
}
