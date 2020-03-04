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
        /// Id da Raspadinha
        /// </summary>
        [Required]
        public int IdScratchcard { get; set; }
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
        public DateTime? Date { get; set; }
        /// <summary>
        /// Data de vencimento da raspadinha
        /// </summary>
        public DateTime? ExpireDate { get; set; }
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
        public DateTime? OpenDate { get; set; }
        /// <summary>
        /// Data que o cliente raspou o bilhete
        /// </summary>
        public DateTime? PlayedDate { get; set; }
        /// <summary>
        /// Data que o cliente validou o bilhete
        /// </summary>
        public DateTime? ValidationDate { get; set; }
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
                this.Prize = scratchcardDraw.Prize;
                this.ValidationCode = scratchcardDraw.ValidationCode;
                this.OpenDate = scratchcardDraw.OpenDate;
                this.PlayedDate = scratchcardDraw.PlayedDate;
                this.ValidationDate = scratchcardDraw.ValidationDate;
                this.Status = scratchcardDraw.Status;
                this.Date = scratchcardDraw.Date;
                this.ExpireDate = scratchcardDraw.ExpireDate;

                if (scratchcardDraw.ScratchcardPrize != null)
                    this.ScratchcardPrize = new ScratchcardPrizeModel(scratchcardDraw.ScratchcardPrize);
            }
        }

        public ScratchcardDraw GetEntity()
        {
            return new ScratchcardDraw()
            {
                Id = this.Id,
                IdScratchcard = this.IdScratchcard,
                IdScratchcardPrize = this.IdScratchcardPrize,
                Image = this.Image,
                IdCustomer = this.IdCustomer,
                Prize = this.Prize,
                ValidationCode = this.ValidationCode,
                OpenDate = this.OpenDate,
                PlayedDate = this.PlayedDate,
                ValidationDate = this.ValidationDate,
                Status = this.Status,
                Date = this.Date,
                ExpireDate = this.ExpireDate
            };
        }
    }
}
