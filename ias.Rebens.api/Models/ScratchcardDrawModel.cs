using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ScratchcardDrawModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id da campanha da Raspadinha
        /// </summary>
        [Required]
        public int IdScratchcard { get; set; }
        /// <summary>
        /// Nome da campanha da raspadinha
        /// </summary>
        public string Scratchcard { get; set; }
        /// <summary>
        /// Id do prêmio se for um bilhete premiado
        /// </summary>
        public int? IdScratchcardPrize { get; set; }
        /// <summary>
        /// Imagem do bilhete
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Image { get; set; }
        /// <summary>
        /// Id do cliente que recebeu esse bilhete
        /// </summary>
        public int? IdCustomer { get; set; }
        /// <summary>
        /// Data da raspadinha
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Data de vencimento da raspadinha
        /// </summary>
        public string ExpireDate { get; set; }
        /// <summary>
        /// Descrição do prêmio se for um bilhete premiado
        /// </summary>
        public string Prize { get; set; }
        /// <summary>
        /// Código de validação
        /// </summary>
        public string ValidationCode { get; set; }
        /// <summary>
        /// Data que o cliente abriu o bilhete
        /// </summary>
        public string OpenDate { get; set; }
        /// <summary>
        /// Data que o cliente raspou o bilhete
        /// </summary>
        public string PlayedDate { get; set; }
        /// <summary>
        /// Data que o cliente validou o bilhete
        /// </summary>
        public string ValidationDate { get; set; }
        /// <summary>
        /// Status do bilhete
        /// </summary>
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public string Instructions { get; set; }

        public CustomerModel Customer { get; set; }

        /// <summary>
        /// Prêmio
        /// </summary>
        public ScratchcardPrizeModel ScratchcardPrize { get; set; }

        public ScratchcardDrawModel() { }

        public ScratchcardDrawModel(ScratchcardDraw scratchcardDraw)
        {
            if (scratchcardDraw != null)
            {
                this.Id = scratchcardDraw.Id;
                this.IdScratchcard = scratchcardDraw.IdScratchcard;
                this.IdScratchcardPrize = scratchcardDraw.IdScratchcardPrize;
                this.Image = scratchcardDraw.Image;
                this.IdCustomer = scratchcardDraw.IdCustomer;
                this.Prize = scratchcardDraw.PlayedDate.HasValue ? scratchcardDraw.Prize : "";
                this.ValidationCode = scratchcardDraw.ValidationCode;
                this.OpenDate = scratchcardDraw.OpenDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.OpenDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.PlayedDate = scratchcardDraw.PlayedDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.PlayedDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.ValidationDate = scratchcardDraw.ValidationDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.ValidationDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.IdStatus = scratchcardDraw.Status;
                this.Date = scratchcardDraw.Date.HasValue ? scratchcardDraw.Date.Value.ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.ExpireDate = scratchcardDraw.ExpireDate.HasValue ? scratchcardDraw.ExpireDate.Value.ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.Status = Enums.EnumHelper.GetEnumDescription((Enums.ScratchcardDraw)scratchcardDraw.Status);
                if (scratchcardDraw.ExpireDate.HasValue && scratchcardDraw.ExpireDate.Value < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Constant.TimeZone).Date && scratchcardDraw.Status == (int)Enums.ScratchcardDraw.drawn)
                {
                    this.Status = "Expirado";
                    this.IdStatus = 99;
                }

                if (scratchcardDraw.ScratchcardPrize != null)
                    this.ScratchcardPrize = new ScratchcardPrizeModel(scratchcardDraw.ScratchcardPrize);
                if (scratchcardDraw.Scratchcard != null)
                    this.Scratchcard = scratchcardDraw.Scratchcard.Name;
                if (scratchcardDraw.Customer != null)
                    this.Customer = new CustomerModel(scratchcardDraw.Customer);
            }
        }

        public ScratchcardDrawModel(ScratchcardDrawItem scratchcardDraw)
        {
            if (scratchcardDraw != null)
            {
                this.Id = scratchcardDraw.Id;
                this.IdScratchcard = scratchcardDraw.IdScratchcard;
                this.IdScratchcardPrize = scratchcardDraw.IdScratchcardPrize;
                this.Image = scratchcardDraw.Image;
                this.IdCustomer = scratchcardDraw.IdCustomer;
                this.Prize = scratchcardDraw.PlayedDate.HasValue ? scratchcardDraw.Prize : "";
                this.ValidationCode = scratchcardDraw.ValidationCode;
                this.OpenDate = scratchcardDraw.OpenDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.OpenDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.PlayedDate = scratchcardDraw.PlayedDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.PlayedDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.ValidationDate = scratchcardDraw.ValidationDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(scratchcardDraw.ValidationDate.Value, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.IdStatus = scratchcardDraw.Status;
                this.Date = scratchcardDraw.Date.HasValue ? scratchcardDraw.Date.Value.ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.ExpireDate = scratchcardDraw.ExpireDate.HasValue ? scratchcardDraw.ExpireDate.Value.ToString("dd/MM/yyyy", Constant.FormatProvider) : "";
                this.Status = Enums.EnumHelper.GetEnumDescription((Enums.ScratchcardDraw)scratchcardDraw.Status);
                this.Instructions = scratchcardDraw.Instructions;
                if (scratchcardDraw.ExpireDate.HasValue && scratchcardDraw.ExpireDate.Value < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Constant.TimeZone).Date && scratchcardDraw.Status == (int)Enums.ScratchcardDraw.drawn)
                {
                    this.Status = "Expirado";
                    this.IdStatus = 99;
                }

                if (scratchcardDraw.ScratchcardPrize != null)
                    this.ScratchcardPrize = new ScratchcardPrizeModel(scratchcardDraw.ScratchcardPrize);
                if (scratchcardDraw.Scratchcard != null)
                    this.Scratchcard = scratchcardDraw.Scratchcard.Name;
                if (scratchcardDraw.Customer != null)
                    this.Customer = new CustomerModel(scratchcardDraw.Customer);
            }
        }
    }
}
