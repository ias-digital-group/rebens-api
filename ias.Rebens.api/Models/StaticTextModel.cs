using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Texto estático
    /// </summary>
    public class StaticTextModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        [MaxLength(200)]
        public string Url { get; set; }
        /// <summary>
        /// Html
        /// </summary>
        [Required]
        public string Html { get; set; }
        /// <summary>
        /// Estilo
        /// </summary>
        [MaxLength(4000)]
        public string Style { get; set; }
        /// <summary>
        /// Ordem
        /// </summary>
        [Required]
        public int Order { get; set; }
        /// <summary>
        /// Id do Tipo de texto estático
        /// </summary>
        [Required]
        public int IdStaticTextType { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Tipo de texto estático
        /// </summary>
        public virtual StaticTextType StaticTextType { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public StaticTextModel() { }

        /// <summary>
        /// Construtor que recebe um objeto StaticText e popula os atributos
        /// </summary>
        /// <param name="staticText"></param>
        public StaticTextModel(StaticText staticText)
        {
            this.Id = staticText.Id;
            this.Title = staticText.Title;
            this.Url = staticText.Url;
            this.Html = staticText.Html;
            this.Style = staticText.Style;
            this.Order = staticText.Order;
            this.IdStaticTextType = staticText.IdStaticTextType;
            this.IdOperation = staticText.IdOperation;
            this.Active = staticText.Active;
        }

        /// <summary>
        /// Retorna um objeto StaticText com as informações
        /// </summary>
        /// <returns></returns>
        public StaticText GetEntity()
        {
            return new StaticText()
            {
                Id = this.Id,
                Title = this.Title,
                Url = this.Url,
                Html = this.Html,
                Style = this.Style,
                Order = this.Order,
                IdStaticTextType = this.IdStaticTextType,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
