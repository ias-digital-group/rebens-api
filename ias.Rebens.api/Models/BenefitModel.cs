using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Benefício
    /// </summary>
    public class BenefitModel
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
        [MaxLength(400)]
        public string Title { get; set; }
        /// <summary>
        /// Imagem
        /// </summary>
        [MaxLength(500)]
        public string Image { get; set; }
        /// <summary>
        /// Data de validade
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Site do benefício
        /// </summary>
        [MaxLength(500)]
        public string WebSite { get; set; }
        /// <summary>
        /// Porcentagem máxima de desconto online
        /// </summary>
        public decimal? MaxDiscountPercentageOnline { get; set; }
        /// <summary>
        /// Porcentagem de CPV online
        /// </summary>
        public decimal? CpvpercentageOnline { get; set; }
        /// <summary>
        /// Porcentagem máxima de desconto offline
        /// </summary>
        public decimal? MaxDiscountPercentageOffline { get; set; }
        /// <summary>
        /// Porcentagem de CPV offline
        /// </summary>
        public decimal? CpvpercentageOffline { get; set; }
        /// <summary>
        /// Início
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Fim
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// Tipo de benefício
        /// </summary>
        [Required]
        public int IdBenefitType { get; set; }
        /// <summary>
        /// É um benefício exclusivo
        /// </summary>
        [Required]
        public bool Exclusive { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Tipo de Integração
        /// </summary>
        [Required]
        public int IdIntegrationType { get; set; }
        /// <summary>
        /// Id do parceiro
        /// </summary>
        [Required]
        public int IdPartner { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public BenefitModel() { }

        /// <summary>
        /// Construtor que recebe um Benefício e já popula os atributos
        /// </summary>
        /// <param name="benefit"></param>
        public BenefitModel(Benefit benefit)
        {
            this.Id = benefit.Id;
            this.Title = benefit.Title;
            this.Image = benefit.Image;
            this.DueDate = benefit.DueDate;
            this.WebSite = benefit.WebSite;
            this.MaxDiscountPercentageOnline = benefit.MaxDiscountPercentageOnline;
            this.CpvpercentageOnline = benefit.CpvpercentageOnline;
            this.MaxDiscountPercentageOffline = benefit.MaxDiscountPercentageOffline;
            this.CpvpercentageOffline = benefit.CpvpercentageOffline;
            this.Start = benefit.Start;
            this.End = benefit.End;
            this.IdBenefitType = benefit.IdBenefitType;
            this.Exclusive = benefit.Exclusive;
            this.Active = benefit.Active;
            this.IdIntegrationType = benefit.IdIntegrationType;
            this.IdPartner = benefit.IdPartner;
        }

        public Benefit GetEntity()
        {
            return new Benefit()
            {
                Id = this.Id,
                Title = this.Title,
                Image = this.Image,
                DueDate = this.DueDate,
                WebSite = this.WebSite,
                MaxDiscountPercentageOnline = this.MaxDiscountPercentageOnline,
                CpvpercentageOnline = this.CpvpercentageOnline,
                MaxDiscountPercentageOffline = this.MaxDiscountPercentageOffline,
                CpvpercentageOffline = this.CpvpercentageOffline,
                Start = this.Start,
                End = this.End,
                IdBenefitType = this.IdBenefitType,
                Exclusive = this.Exclusive,
                Active = this.Active,
                IdIntegrationType = this.IdIntegrationType,
                IdPartner = this.IdPartner
        };
        }
    }
}
