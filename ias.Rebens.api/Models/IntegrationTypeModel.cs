using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class IntegrationTypeModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public IntegrationTypeModel() { }
        public IntegrationTypeModel(IntegrationType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
