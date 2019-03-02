using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Tipo de Texto Estático
    /// </summary>
    public class StaticTextTypeModel
    {
        /// <summary>
        /// Id
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
        public StaticTextTypeModel() { }

        /// <summary>
        /// Construtor que recebe um objeto StaticTextType e popula os atributos
        /// </summary>
        /// <param name="type"></param>
        public StaticTextTypeModel(Enums.StaticTextType type)
        {
            this.Id = (int)type;
            this.Name = Enums.EnumHelper.GetEnumDescription(type);
        }
    }
}
