using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Banner
    /// </summary>
    public class BannerModel
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
        /// Imagem
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Image { get; set; }
        /// <summary>
        /// Ordem
        /// </summary>
        [Required]
        public int Order { get; set; }
        /// <summary>
        /// Link
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Link { get; set; }
        /// <summary>
        /// Tipo
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Cor de fundo
        /// </summary>
        [MaxLength(50)]
        public string BackgroundColor { get; set; }
        /// <summary>
        /// Id do benefício
        /// </summary>
        public int? IdBenefit { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Data de inicio
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Data fim
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public BannerModel() { }

        /// <summary>
        /// Construtor que recebe um Banner e popula os atributos
        /// </summary>
        /// <param name="banner"></param>
        public BannerModel(Banner banner)
        {
            this.Id = banner.Id;
            this.Name = banner.Name;
            this.Image = banner.Image;
            this.Order = banner.Order;
            this.Link = banner.Link;
            this.Type = banner.Type;
            this.BackgroundColor = banner.BackgroundColor;
            this.IdBenefit = banner.IdBenefit;
            this.Active = banner.Active;
            this.Start = banner.Start;
            this.End = banner.End;
        }

        /// <summary>
        /// Retorna um objeto Banner com as informações
        /// </summary>
        /// <returns></returns>
        public Banner GetEntity()
        {
            return new Banner()
            {
                Id = this.Id,
                Name = this.Name,
                Image = this.Image,
                Order = this.Order,
                Link = this.Link,
                Type = this.Type,
                BackgroundColor = this.BackgroundColor,
                IdBenefit = this.IdBenefit,
                Active = this.Active,
                Start = this.Start,
                End = this.End,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }

    public class BannerTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
