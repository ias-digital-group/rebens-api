using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

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
        /// Se está ativa ou inativa
        /// </summary>
        [Required]
        public bool Active { get; set; }
    
        /// <summary>
        /// O tipo de category (benefícios = 4, cursos livres = 19)
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Lista de categorias filhas, caso seja uma categoria pai
        /// </summary>
        public List<CategoryModel> Categories { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }
        /// <summary>
        /// Indica se a categoria possui filhos vinculados
        /// </summary>
        public bool HasChild { get; }
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
            if (category != null)
            {
                this.Id = category.Id;
                this.Name = category.Name;
                this.IdParent = category.IdParent;
                this.Order = category.Order;
                this.Active = category.Active;
                this.Type = category.Type;

                this.Categories = new List<CategoryModel>();
                if (category.Categories != null)
                {
                    foreach (var cat in category.Categories.OrderBy(c => c.Name))
                        this.Categories.Add(new CategoryModel(cat));
                }
            }
        }

        public CategoryModel(CategoryItem category)
        {
            if (category != null)
            {
                this.Id = category.Id;
                this.Name = category.Name;
                this.IdParent = category.IdParent;
                this.Order = category.Order;
                this.Active = category.Active;
                this.Type = category.Type;
                this.HasChild = category.HasChild;

                this.Categories = new List<CategoryModel>();
                if (category.Categories != null)
                {
                    foreach (var cat in category.Categories.OrderBy(c => c.Name))
                        this.Categories.Add(new CategoryModel(cat));
                }
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
                IdParent = this.IdParent,
                Id = this.Id,
                Name = this.Name,
                Order = this.Order,
                Type = this.Type
            };
        }
    }


    public class CategoryListItemModel
    {
        /// <summary>
        /// Id da categoria
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id do pai, caso seja uma subcategoria, caso contrário null
        /// </summary>
        public int? IdParent { get; set; }
        /// <summary>
        /// Se está ativa ou inativa
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Data da criação
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// Nome do ususário que criou
        /// </summary>
        public string CreatedUserName { get; set; }
        /// <summary>
        /// Data da Modificação
        /// </summary>
        public string Modified { get; set; }
        /// <summary>
        /// Nome do último usuário que modificou
        /// </summary>
        public string ModifiedUserName { get; set; }
        /// <summary>
        /// O tipo de category (benefícios = 1, cursos livres = 2)
        /// </summary>
        public string Type { get; set; }

        public CategoryListItemModel() { }

        public CategoryListItemModel(Entity.CategoryListItem category)
        {
            if (category != null)
            {
                this.Id = category.Id;
                this.Name = category.Name;
                this.IdParent = category.IdParent;
                this.Active = category.Active;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(category.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.CreatedUserName = category.CreatedUserName;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(category.Modified, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.ModifiedUserName = category.ModifiedUserName;
                this.Type = Enums.EnumHelper.GetEnumDescription((Enums.LogItem)category.IdType);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CategoryItemModel : CategoryModel
    {
        /// <summary>
        /// Retorna se a categoria está selecionada
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CategoryItemModel() { }

        /// <summary>
        /// Construtor que já popula as propriedades
        /// </summary>
        public CategoryItemModel(CategoryItem category) {
            this.Id = category.Id;
            this.Name = category.Name;
            this.IdParent = category.IdParent;
            this.Order = category.Order;
            this.Active = category.Active;
            this.Checked = category.IdBenefit.HasValue;
        }
    }
}
