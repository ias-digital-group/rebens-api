using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ScratchcardModel
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
        /// Inicio 
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Fim
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// Quantidade
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// Imagem sem prêmio 1
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage1 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 2
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage2 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 3
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage3 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 4
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage4 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 5
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage5 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 6
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage6 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 7
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage7 { get; set; }
        /// <summary>
        /// Imagem sem prêmio 8
        /// </summary>
        [MaxLength(500)]
        public string NoPrizeImage8 { get; set; }
        /// <summary>
        /// Id da Operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Status 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Prêmios
        /// </summary>
        public List<ScratchcardPrizeModel> Prizes { get; }

        public ScratchcardModel() { }
        
        public ScratchcardModel(Scratchcard scratchcard) 
        {
            if (scratchcard != null)
            {
                this.Id = scratchcard.Id;
                this.Name = scratchcard.Name;
                this.Start = scratchcard.Start;
                this.End = scratchcard.End;
                this.Quantity = scratchcard.Quantity;
                this.NoPrizeImage1 = scratchcard.NoPrizeImage1;
                this.NoPrizeImage2 = scratchcard.NoPrizeImage2;
                this.NoPrizeImage3 = scratchcard.NoPrizeImage3;
                this.NoPrizeImage4 = scratchcard.NoPrizeImage4;
                this.NoPrizeImage5 = scratchcard.NoPrizeImage5;
                this.NoPrizeImage6 = scratchcard.NoPrizeImage6;
                this.NoPrizeImage7 = scratchcard.NoPrizeImage7;
                this.NoPrizeImage8 = scratchcard.NoPrizeImage8;
                this.IdOperation = scratchcard.IdOperation;
                this.Status = scratchcard.Status;

                if(scratchcard.Prizes != null)
                {
                    this.Prizes = new List<ScratchcardPrizeModel>();
                    foreach (var prize in scratchcard.Prizes)
                        this.Prizes.Add(new ScratchcardPrizeModel(prize));
                }
            }
        }

        public Scratchcard GetEntity()
        {
            return new Scratchcard()
            {
                Id = this.Id,
                Name = this.Name,
                Start = this.Start,
                End = this.End,
                Quantity = this.Quantity,
                NoPrizeImage1 = this.NoPrizeImage1,
                NoPrizeImage2 = this.NoPrizeImage2,
                NoPrizeImage3 = this.NoPrizeImage3,
                NoPrizeImage4 = this.NoPrizeImage4,
                NoPrizeImage5 = this.NoPrizeImage5,
                NoPrizeImage6 = this.NoPrizeImage6,
                NoPrizeImage7 = this.NoPrizeImage7,
                NoPrizeImage8 = this.NoPrizeImage8,
                IdOperation = this.IdOperation,
                Status = this.Status
            };
        }
    }
}
