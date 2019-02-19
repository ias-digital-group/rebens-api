using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        /// Id Tipo de benefício
        /// </summary>
        [Required]
        public int IdBenefitType { get; set; }
        /// <summary>
        /// Tipo de benefício
        /// </summary>
        public string BenefitType { get; set; }
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
        /// Id Tipo de Integração
        /// </summary>
        [Required]
        public int IdIntegrationType { get; set; }
        /// <summary>
        /// Id do parceiro
        /// </summary>
        [Required]
        public int IdPartner { get; set; }
        /// <summary>
        /// Logo do Parceiro
        /// </summary>
        public string PartnerImage { get; set; }
        /// <summary>
        /// Tipo de Integração
        /// </summary>
        public string IntegrationType { get; set; }
        /// <summary>
        /// Chamada do Benefício
        /// </summary>
        public string BenefitCall { get; set; }
        /// <summary>
        /// Detalhes
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// Como Utilizar
        /// </summary>
        public string HowToUse { get; set; }

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
            if (benefit.BenefitType != null)
                this.BenefitType = benefit.BenefitType.Name;
            if (benefit.IntegrationType != null)
                this.IntegrationType = benefit.IntegrationType.Name;
            if(benefit.StaticTexts != null)
            {
                foreach(var text in benefit.StaticTexts)
                {
                    switch((Enums.StaticTextType)text.IdStaticTextType)
                    {
                        case Enums.StaticTextType.BenefitCall:
                            this.BenefitCall = text.Html;
                            break;
                        case Enums.StaticTextType.BenefitDetail:
                            this.Detail = text.Html;
                            break;
                        case Enums.StaticTextType.BenefitHowToUse:
                            this.HowToUse = text.Html;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StaticText GetCall()
        {
            return new StaticText()
            {
                Active = true,
                Created = DateTime.Now,
                Html = this.BenefitCall,
                IdBenefit = this.Id,
                IdStaticTextType = (int)Enums.StaticTextType.BenefitCall,
                Modified = DateTime.Now,
                Order = 1,
                Title = "Chamada - " + this.Title
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StaticText GetDetail()
        {
            return new StaticText()
            {
                Active = true,
                Created = DateTime.Now,
                Html = this.Detail,
                IdBenefit = this.Id,
                IdStaticTextType = (int)Enums.StaticTextType.BenefitDetail,
                Modified = DateTime.Now,
                Order = 1,
                Title = "Detalhe - " + this.Title
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StaticText GetHowToUse()
        {
            return new StaticText()
            {
                Active = true,
                Created = DateTime.Now,
                Html = this.HowToUse,
                IdBenefit = this.Id,
                IdStaticTextType = (int)Enums.StaticTextType.BenefitHowToUse,
                Modified = DateTime.Now,
                Order = 1,
                Title = "Como Utilizar - " + this.Title
            };
        }
    }

    /// <summary>
    /// Benefit List Item
    /// </summary>
    public class BenefitListItem
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
        /// Imagem
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Id Tipo de benefício
        /// </summary>
        public int IdBenefitType { get; set; }
        /// <summary>
        /// Tipo de benefício
        /// </summary>
        public string BenefitType { get; set; }
        /// <summary>
        /// Chamada do Benefício
        /// </summary>
        public string BenefitCall { get; set; }
        /// <summary>
        /// Construtor
        /// </summary>
        public BenefitListItem() { }
        /// <summary>
        /// Construtor que recebe um Benefício e já popula os atributos
        /// </summary>
        /// <param name="benefit"></param>
        public BenefitListItem(Benefit benefit)
        {
            this.Id = benefit.Id;
            this.Title = benefit.Title;
            this.IdBenefitType = benefit.IdBenefitType;
            if (benefit.StaticTexts != null)
            {
                var staticText = benefit.StaticTexts.SingleOrDefault(s => s.IdStaticTextType == (int)Enums.StaticTextType.BenefitCall);
                if(staticText != null && staticText.Id > 0)
                    this.BenefitCall = staticText.Html;
            }
            if(benefit.BenefitType != null)
                this.BenefitType = benefit.BenefitType.Name;
            if (benefit.Partner != null)
                this.Image = benefit.Partner.Logo;
        }
    }

    /// <summary>
    /// Item simplificado
    /// </summary>
    public class BenefitItem
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
        /// Construtor
        /// </summary>
        public BenefitItem() { }

        /// <summary>
        /// Construtor que recebe um Benefício e já popula os atributos
        /// </summary>
        /// <param name="benefit"></param>
        public BenefitItem(Benefit benefit)
        {
            this.Id = benefit.Id;
            this.Title = benefit.Title;
        }
    }
}
