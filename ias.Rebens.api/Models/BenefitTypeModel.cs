using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class BenefitTypeModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public BenefitTypeModel() { }
        public BenefitTypeModel(BenefitType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
