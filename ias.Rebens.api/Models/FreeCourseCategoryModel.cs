using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class FreeCourseCategoryModel
    {
        /// <summary>
        /// Id do curso livre
        /// </summary>
        [Required]
        public int IdFreeCourse { get; set; }

        /// <summary>
        /// Id da categoria
        /// </summary>
        [Required]
        public int IdCategory { get; set; }
    }

    /// <summary>
    /// Relacionamento Curso Livre com Categoria
    /// </summary>
    public class FreeCourseCategoriesModel
    {
        /// <summary>
        /// Id do curso livre
        /// </summary>
        [Required]
        public int IdFreeCourse { get; set; }

        /// <summary>
        /// ids das categorias selecionadas separado por ','
        /// </summary>
        public string CategoryIds { get; set; }
    }
}
