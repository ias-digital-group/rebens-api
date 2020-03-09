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
        public int Status { get; set; }

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
                this.OpenDate = scratchcardDraw.OpenDate.HasValue ? scratchcardDraw.OpenDate.Value.ToString("dd/MM/yyyy") : "";
                this.PlayedDate = scratchcardDraw.PlayedDate.HasValue ? scratchcardDraw.PlayedDate.Value.ToString("dd/MM/yyyy") : "";
                this.ValidationDate = scratchcardDraw.ValidationDate.HasValue ? scratchcardDraw.ValidationDate.Value.ToString("dd/MM/yyyy") : "";
                this.Status = scratchcardDraw.Status;
                this.Date = scratchcardDraw.Date.HasValue ? scratchcardDraw.Date.Value.ToString("dd/MM/yyyy") : "";
                this.ExpireDate = scratchcardDraw.ExpireDate.HasValue ? scratchcardDraw.ExpireDate.Value.ToString("dd/MM/yyyy") : "";

                if (scratchcardDraw.ScratchcardPrize != null)
                    this.ScratchcardPrize = new ScratchcardPrizeModel(scratchcardDraw.ScratchcardPrize);
                if (scratchcardDraw.Scratchcard != null)
                    this.Scratchcard = scratchcardDraw.Scratchcard.Name;
            }
        }
    }
}
