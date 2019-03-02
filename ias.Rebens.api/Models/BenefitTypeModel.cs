using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Tipo de benefício
    /// </summary>
    public class BenefitTypeModel
    {
        /// <summary>
        /// Id do tipo de benefício
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
        /// Construtor
        /// </summary>
        public BenefitTypeModel() { }

        /// <summary>
        /// Construtor que recebe um BenefitType e popula os atributos
        /// </summary>
        /// <param name="type"></param>
        public BenefitTypeModel(Enums.BenefitType type)
        {
            this.Id = (int)type;
            this.Name = Enums.EnumHelper.GetEnumDescription(type);
        }
    }
}
