using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Portal Banner model 
    /// </summary>
    public class PortalBannerModel
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
        /// Target
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Chamada do Benefício
        /// </summary>
        public string BenefitCall { get; set; }
        /// <summary>
        /// Id do benefício
        /// </summary>
        public int? IdBenefit { get; set; }
        /// <summary>
        /// Logo do Parceiro
        /// </summary>
        public string PartnerLogo { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public PortalBannerModel() { }

        /// <summary>
        /// Construtor que recebe um Banner e popula os atributos
        /// </summary>
        /// <param name="banner"></param>
        /// <param name="title"></param>
        /// <param name="benefitCall"></param>
        /// <param name="partnerLogo"></param>
        public PortalBannerModel(Banner banner, string title, string benefitCall, string partnerLogo)
        {
            this.Id = banner.Id;
            this.Name = banner.Name;
            this.Image = banner.Image;
            this.Order = banner.Order;
            this.Link = banner.Link;
            this.Type = banner.Type;
            this.Title = string.IsNullOrEmpty(benefitCall) ? title : (string.IsNullOrEmpty(title) ? "" : title + " | ");
            this.Target = banner.Target;
            this.BenefitCall = benefitCall;
            this.PartnerLogo = partnerLogo;
            this.IdBenefit = banner.IdBenefit;
        }
    }
}
