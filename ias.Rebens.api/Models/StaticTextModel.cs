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
        /// Id do tipo de texto estático
        /// </summary>
        public int IdStaticTextType { get; set; }
        /// <summary>
        /// Nome da operação
        /// </summary>
        public string OperationName { get; set; }

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
            if (staticText != null)
            {
                this.Id = staticText.Id;
                this.Name = staticText.Title;
                this.Page = staticText.Url;
                if (!string.IsNullOrEmpty(staticText.Html) && staticText.Html.StartsWith('{'))
                {
                    try
                    {
                        this.Data = JObject.Parse(staticText.Html);
                    }
                    catch
                    {
                        this.Data = staticText.Html;
                    }
                }
                else
                    this.Data = staticText.Html;
                this.IdOperation = staticText.IdOperation.HasValue ? staticText.IdOperation.Value : 0;
                this.Active = staticText.Active;
                this.IdStaticTextType = staticText.IdStaticTextType;
                if (staticText.Operation != null)
                    this.OperationName = staticText.Operation.Title;
            }
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
                Html = this.Data.ToString().StartsWith("{") ? JsonConvert.SerializeObject(this.Data) : this.Data.ToString(),
                Style = "",
                Order = 0,
                IdStaticTextType = this.IdStaticTextType == 0 ? (int)Enums.StaticTextType.Pages : this.IdStaticTextType,
                IdOperation = this.IdOperation != 0 ? (int?)this.IdOperation : null,
                IdBenefit = null,
                Active = this.Active,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }

    public class StaticTextListItemModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Html ou objeto em json
        /// </summary>
        public string Html { get; set; }
        /// <summary>
        /// Style ou objeto em json
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// Ordem
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Id do Tipo
        /// </summary>
        public int IdStaticTextType { get; set; }
        /// <summary>
        /// Tipo
        /// </summary>
        public string Type { get { return Enums.EnumHelper.GetEnumDescription((Enums.StaticTextType)this.IdStaticTextType); } }
        /// <summary>
        /// Id da operação
        /// </summary>
        public int IdOperation { get; set; }
        /// <summary>
        /// Nome da operação
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// Logo da operação
        /// </summary>
        public string OperationLogo { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Data cadastro
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// Data última modificação
        /// </summary>
        public string Modified { get; set; }
        /// <summary>
        /// Id do benefício
        /// </summary>
        public int? IdBenefit { get; set; }
        /// <summary>
        /// Usuário que criou
        /// </summary>
        public string CreatedUserName { get; set; }
        /// <summary>
        /// último usuário que fez alguma alteração
        /// </summary>
        public string ModifiedUserName { get; set; }

        public StaticTextListItemModel() { }

        public StaticTextListItemModel(Entity.StaticTextListItem text) {
            if (text != null)
            {
                this.Active = text.Active;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(text.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.CreatedUserName = text.CreatedUserName;
                this.Html = text.Html;
                this.Id = text.Id;
                this.IdBenefit = text.IdBenefit;
                this.IdOperation = text.IdOperation;
                this.IdStaticTextType = text.IdStaticTextType;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(text.Modified, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.ModifiedUserName = text.ModifiedUserName;
                this.OperationLogo = text.OperationLogo;
                this.OperationName = text.OperationName;
                this.Order = text.Order;
                this.Style = text.Style;
                this.Title = text.Title;
                this.Url = text.Url;
            }
        }
    }
}
