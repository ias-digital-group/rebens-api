using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ias.Rebens.api.Models
{
    public class CategoryModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public int Order { get; set; }
        public int? IdParent { get; set; }
        [MaxLength(500)]
        public string Icon { get; set; }
        [Required]
        public bool Active { get; set; }
    
        public List<CategoryModel> Categories { get; set; }

        public CategoryModel() { }
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
