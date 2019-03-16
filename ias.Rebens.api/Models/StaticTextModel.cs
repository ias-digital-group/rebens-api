using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        /// Página
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Page { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Objeto a ser salvo
        /// </summary>
        [Required]
        public object Data { get; set; }
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
            this.Name = staticText.Title;
            this.Page = staticText.Url;
            this.Data = JObject.Parse(staticText.Html);
            this.IdOperation = staticText.IdOperation.Value;
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
                Title = this.Name,
                Url = this.Page,
                Html = JsonConvert.SerializeObject(this.Data),
                Style = "",
                Order = 0,
                IdStaticTextType = (int)Enums.StaticTextType.Pages,
                IdOperation = this.IdOperation,
                IdBenefit = null,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
