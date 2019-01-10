using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Categoria
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// Id da categoria
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
        /// Ordem que ela deve aparecer
        /// </summary>
        [Required]
        public int Order { get; set; }

        /// <summary>
        /// Id do pai, caso seja uma subcategoria, caso contrário null
        /// </summary>
        public int? IdParent { get; set; }

        /// <summary>
        /// Ícone, nome da imagem representando o ícone
        /// </summary>
        [MaxLength(500)]
        public string Icon { get; set; }

        /// <summary>
        /// Se está ativa ou inativa
        /// </summary>
        [Required]
        public bool Active { get; set; }
    
        /// <summary>
        /// Lista de categorias filhas, caso seja uma categoria pai
        /// </summary>
        public List<CategoryModel> Categories { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CategoryModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Category e já popula os atributos
        /// </summary>
        /// <param name="category"></param>
        public CategoryModel(Category category)
        {
            this.Id = category.Id;
            this.Name = category.Name;
            this.IdParent = category.IdParent;
            this.Order = category.Order;
            this.Icon = category.Icon;
            this.Active = category.Active;

            this.Categories = new List<CategoryModel>();

            if (category.Categories != null)
            {
                foreach (var cat in category.Categories)
                    this.Categories.Add(new CategoryModel(cat));
            }
        }

        /// <summary>
        /// Retorna um objeto Categoria com as informações
        /// </summary>
        /// <returns></returns>
        public Category GetEntity() {
            return new Category()
            {
                Active = this.Active,
                Icon = this.Icon,
                IdParent = this.IdParent,
                Id = this.Id,
                Name = this.Name,
                Order = this.Order
            };
        }
    }
}
