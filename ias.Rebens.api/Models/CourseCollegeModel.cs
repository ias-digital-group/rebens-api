using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Faculdade
    /// </summary>
    public class CourseCollegeModel
    {
        /// <summary>
        /// Id da Faculdade
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome da Faculdade
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Se a Faculdade está ativa ou não
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Endereços da Faculdade
        /// </summary>
        public List<AddressModel> Addresses { get; set; }

        /// <summary>
        /// Endereço da Faculdade
        /// </summary>
        public AddressModel Address { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CourseCollegeModel() { }

        /// <summary>
        /// Construtor que recebe um objeto CourseGraduationType e popula os atributos
        /// </summary>
        /// <param name="courseCollege"></param>
        public CourseCollegeModel(CourseCollege courseCollege)
        {
            this.Id = courseCollege.Id;
            this.Name = courseCollege.Name;
            this.Active = courseCollege.Active;
            this.Logo = courseCollege.Logo;
            this.IdOperation = courseCollege.IdOperation;
        }

        /// <summary>
        /// Retorna um objeto CourseCollege com as informações
        /// </summary>
        /// <returns></returns>
        public CourseCollege GetEntity()
        {
            return new CourseCollege()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Logo = this.Logo,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Relacionamento Faculdade com Endereço
    /// </summary>
    public class CourseCollegeAddressModel
    {
        /// <summary>
        /// Id da faculdade
        /// </summary>
        [Required]
        public int IdCourseCollege { get; set; }


        /// <summary>
        /// Id do endereço
        /// </summary>
        [Required]
        public int IdAddress { get; set; }
    }
}
