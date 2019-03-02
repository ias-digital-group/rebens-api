using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Tipo de integração
    /// </summary>
    public class IntegrationTypeModel
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
        public IntegrationTypeModel() { }

        /// <summary>
        /// Construtor que recebe um objeto IntegrationType, e popula os atributos
        /// </summary>
        /// <param name="type"></param>
        public IntegrationTypeModel(Enums.IntegrationType type)
        {
            this.Id = (int)type;
            this.Name = Enums.EnumHelper.GetEnumDescription(type);
        }
    }
}
