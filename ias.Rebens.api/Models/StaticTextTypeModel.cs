using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class StaticTextTypeModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public StaticTextTypeModel() { }
        public StaticTextTypeModel(StaticTextType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
