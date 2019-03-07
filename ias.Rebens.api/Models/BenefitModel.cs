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
        [MaxLength(300)]
        public string Name { get; set; }
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
        /// Porcentagem máxima de desconto
        /// </summary>
        public decimal? MaxDiscountPercentage { get; set; }
        /// <summary>
        /// Porcentagem de CPV
        /// </summary>
        public decimal? Cpvpercentage { get; set; }
        /// <summary>
        /// Porcentagem mínima de desconto
        /// </summary>
        public decimal? MinDiscountPercentage { get; set; }
        /// <summary>
        /// Valor do cashback
        /// </summary>
        public decimal? CashbackAmount { get; set; }
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
        public string BenefitType { get { return Enums.EnumHelper.GetEnumDescription((Enums.BenefitType)this.IdBenefitType); } }
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
        /// Descrição do Parceiro
        /// </summary>
        public string PartnerDescription { get; set; }
        /// <summary>
        /// Tipo de Integração
        /// </summary>
        public string IntegrationType { get { return Enums.EnumHelper.GetEnumDescription((Enums.IntegrationType)this.IdIntegrationType); } }
        /// <summary>
        /// Chamada do Benefício
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Call { get; set; }
        /// <summary>
        /// Detalhes
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// Como Utilizar
        /// </summary>
        public string HowToUse { get; set; }
        /// <summary>
        /// Link do benefício
        /// </summary>
        [MaxLength(500)]
        public string Link { get; set; }
        /// <summary>
        /// Texto do Voucher
        /// </summary>
        [MaxLength(500)]
        public string VoucherText { get; set; }
        /// <summary>
        /// Id da operação quando o benefício é exclusivo
        /// </summary>
        public int? IdOperation { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }
        /// <summary>
        /// Construtor
        /// </summary>
        public BenefitModel() { }

        /// <summary>
        /// Construtor que recebe um Benefício e já popula os atributos
        /// </summary>
        /// <param name="benefit"></param>
        /// <param name="idCustomer"></param>
        public BenefitModel(Benefit benefit, int? idCustomer = null)
        {
            this.Id = benefit.Id;
            this.Name = benefit.Name;
            this.Title = benefit.Title;
            this.Image = benefit.Image;
            this.DueDate = benefit.DueDate;
            this.Link = benefit.Link;
            this.MaxDiscountPercentage = benefit.MaxDiscountPercentage;
            this.Cpvpercentage = benefit.CPVPercentage;
            this.MinDiscountPercentage = benefit.MinDiscountPercentage;
            this.CashbackAmount = benefit.CashbackAmount;
            this.Start = benefit.Start;
            this.End = benefit.End;
            this.IdBenefitType = benefit.IdBenefitType;
            this.Exclusive = benefit.Exclusive;
            this.Active = benefit.Active;
            this.IdIntegrationType = benefit.IdIntegrationType;
            this.IdPartner = benefit.IdPartner;
            this.Link = benefit.Link;
            this.Call = benefit.Call;
            this.VoucherText = benefit.VoucherText;
            this.IdOperation = benefit.IdOperation;

            if (this.IdBenefitType == (int)Enums.BenefitType.OffLine && idCustomer.HasValue)
                this.Link = "http://admin.rebens.com.br/Voucher/?code=" + System.Web.HttpUtility.UrlEncode(Helper.SecurityHelper.SimpleEncryption(this.Id + "|" + idCustomer.Value));
            if (this.IdBenefitType == (int)Enums.BenefitType.Cashback && idCustomer.HasValue)
                this.Link = benefit.Link + (benefit.Link.IndexOf('?') > 0 ? "&" : "?") + "zpar0=" + System.Web.HttpUtility.UrlEncode(Helper.SecurityHelper.SimpleEncryption(this.Id + "|" + idCustomer.Value));

            if (benefit.Partner != null)
            {
                this.PartnerImage = benefit.Partner.Logo;
                if(benefit.Partner.StaticText != null)
                    this.PartnerDescription = benefit.Partner.StaticText.Html;
            }

            if (benefit.StaticTexts != null)
            {
                foreach(var text in benefit.StaticTexts)
                {
                    switch((Enums.StaticTextType)text.IdStaticTextType)
                    {
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
                Name = this.Name,
                Title = this.Title,
                Image = this.Image,
                DueDate = this.DueDate,
                Link = this.Link,
                MaxDiscountPercentage = this.MaxDiscountPercentage,
                CPVPercentage = this.Cpvpercentage,
                MinDiscountPercentage = this.MinDiscountPercentage,
                CashbackAmount = this.CashbackAmount,
                Start = this.Start,
                End = this.End,
                IdBenefitType = this.IdBenefitType,
                Exclusive = this.Exclusive,
                Active = this.Active,
                IdIntegrationType = this.IdIntegrationType,
                IdPartner = this.IdPartner,
                Call = this.Call,
                VoucherText = this.VoucherText,
                IdOperation = this.IdOperation
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
        public string Call { get; set; }
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
            this.Call = benefit.Call;
            this.BenefitType = Enums.EnumHelper.GetEnumDescription((Enums.BenefitType)benefit.IdBenefitType);
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
        /// Título
        /// </summary>
        [Required]
        [MaxLength(400)]
        public string Name { get; set; }


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
            this.Name = benefit.Name;
        }
    }
}
