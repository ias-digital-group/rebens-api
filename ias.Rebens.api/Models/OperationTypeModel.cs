using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class OperationTypeModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public OperationTypeModel() { }
        public OperationTypeModel(OperationType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
