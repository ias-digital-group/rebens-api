using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ScratchcardPrizeModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id Raspadinha
        /// </summary>
        [Required]
        public int IdScratchcard { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Título
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        /// <summary>
        /// Imagem do prêmio
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Image { get; set; }
        /// <summary>
        /// Quantidade de bilhetes premiados
        /// </summary>
        [Required]
        public int Quantity { get; set; }
        /// <summary>
        /// Descrição do prêmio
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        /// <summary>
        /// Chances
        /// </summary>
        public decimal Odds { get; set; }

        public ScratchcardPrizeModel() { }

        public ScratchcardPrizeModel(ScratchcardPrize scratchcardPrize)
        {
            if (scratchcardPrize != null)
            {
                this.Id = scratchcardPrize.Id;
                this.IdScratchcard = scratchcardPrize.IdScratchcard;
                this.Name = scratchcardPrize.Name;
                this.Title = scratchcardPrize.Title;
                this.Image = scratchcardPrize.Image;
                this.Quantity = scratchcardPrize.Quantity;
                this.Description = scratchcardPrize.Description;
                this.Odds = scratchcardPrize.Odds;
            }
        }

        public ScratchcardPrize GetEntity()
        {
            return new ScratchcardPrize()
            {
                Id = this.Id,
                IdScratchcard = this.IdScratchcard,
                Name = this.Name,
                Title = this.Title,
                Image = this.Image,
                Quantity = this.Quantity,
                Description = this.Description,
                Odds = this.Odds
            };
        }
    }
}
