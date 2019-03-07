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
        [MaxLength(500)]
        public string Link { get; set; }
        /// <summary>
        /// id Tipo
        /// </summary>
        [Required]
        public int IdType { get; set; }
        /// <summary>
        /// Tipo
        /// </summary>
        public string Type { get; set; }
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
        /// é de benefício
        /// </summary>
        public bool IsBenefit { get { return this.IdBenefit.HasValue; } }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Data de inicio
        /// </summary>
        [Required]
        public DateTime Start { get; set; }
        /// <summary>
        /// Data fim
        /// </summary>
        [Required]
        public DateTime End { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }
        /// <summary>
        /// aparece na home
        /// </summary>
        public bool BannerShowHome { get; set; }
        /// <summary>
        /// aparece na home
        /// </summary>
        public bool BannerShowHomeLogged { get; set; }
        /// <summary>
        /// aparece na home
        /// </summary>
        public bool BannerShowBenefit { get; set; }
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
            this.IdType = banner.Type;
            this.Type = Enums.EnumHelper.GetEnumDescription((Enums.BannerType)banner.Type);
            this.BackgroundColor = banner.BackgroundColor;
            this.IdBenefit = banner.IdBenefit;
            this.Active = banner.Active;
            this.Start = banner.Start.Value;
            this.End = banner.End.Value;
            this.BannerShowHome = (banner.IdBannerShow & (int)Enums.BannerShow.HomeNotLogged) == (int)Enums.BannerShow.HomeNotLogged;
            this.BannerShowHomeLogged = (banner.IdBannerShow & (int)Enums.BannerShow.HomeLogged) == (int)Enums.BannerShow.HomeLogged;
            this.BannerShowBenefit = (banner.IdBannerShow & (int)Enums.BannerShow.Benefit) == (int)Enums.BannerShow.Benefit;
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
                Type = this.IdType,
                BackgroundColor = this.BackgroundColor,
                IdBenefit = this.IdBenefit,
                Active = this.Active,
                Start = this.Start,
                End = this.End,
                IdBannerShow = (this.BannerShowHome ? (int)Enums.BannerShow.HomeNotLogged : 0) + 
                    (this.BannerShowHomeLogged ? (int)Enums.BannerShow.HomeLogged : 0) +
                    (this.BannerShowBenefit ? (int)Enums.BannerShow.Benefit : 0),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Associação Banner Operação
    /// </summary>
    public class BannerOperationModel
    {
        /// <summary>
        /// Id do banner
        /// </summary>
        public int IdBanner { get; set; }
        /// <summary>
        /// Id da operação
        /// </summary>
        public int IdOperation { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BannerOperationItemModel : BannerOperationModel
    {
        /// <summary>
        /// Nome da operação
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Se a operação está vinculada com o benefício
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Construtor 
        /// </summary>
        public BannerOperationItemModel()
        {
        }

        /// <summary>
        /// Construtor que já populas as propriedades
        /// </summary>
        /// <param name="item"></param>
        public BannerOperationItemModel(BannerOperationItem item)
        {
            this.IdBanner = item.IdBanner ?? 0;
            this.IdOperation = item.IdOperation;
            this.OperationName = item.OperationName;
            this.Checked = item.IdBanner.HasValue;
        }
    }

    /// <summary>
    /// tipo de banner
    /// </summary>
    public class BannerTypeModel
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
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
