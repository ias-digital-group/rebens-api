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
        /// Tipo de campanha (Aberto = 1, Fechado = 2, Fechado + parceiro = 3, Assinatura = 4)
        /// </summary>
        [Required]
        public int Type { get; set; }
        /// <summary>
        /// Tipo de distribuição (diária = 1, semanal = 2, mensal = 3)
        /// </summary>
        [Required]
        public int DistributionType { get; set; }
        /// <summary>
        /// Quantidade de bilhetes à serem distribuidos (para diário esse campo é ignorado, para semanal no máximo 6 e para mensal no máximo 31)
        /// </summary>
        public int? DistributionQuantity { get; set; }
        /// <summary>
        /// Se o bilhete expira no dia que ele é emitido, ou seja, o cliente precisa raspara o bilhete no mesmo dia
        /// </summary>
        [Required]
        public bool ScratchcardExpire { get; set; }
        /// <summary>
        /// Informações sobre como resgatar os prêmios, vai aparecer no modal da raspadinha
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// Se o usuário quer receber notificações das raspadinhas com prêmio
        /// </summary>
        [Required]
        public bool GetNotifications { get; set; }
        /// <summary>
        /// Id do usuário que criou a campanha
        /// </summary>
        public int IdAdminUser { get; set; }

        public string StatusName { get; set; }
        public string Operation { get; set; }
        public bool CanEdit { get; set; }
        public bool CanPublish { get; set; }
        public string ImagesPath { get; set; }
        public string Regulation { get; set; }

        /// <summary>
        /// Prêmios
        /// </summary>
        public List<ScratchcardPrizeModel> Prizes { get; }

        public ScratchcardModel() { }
        
        public ScratchcardModel(Scratchcard scratchcard, string operationName = null) 
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
                this.Type = scratchcard.Type;
                this.DistributionQuantity = scratchcard.DistributionQuantity;
                this.DistributionType = scratchcard.DistributionType;
                this.ScratchcardExpire = scratchcard.ScratchcardExpire;
                this.IdOperation = scratchcard.IdOperation;
                this.Status = scratchcard.Status;
                this.Operation = operationName;
                this.StatusName = Enums.EnumHelper.GetEnumDescription((Enums.ScratchcardStatus)scratchcard.Status);
                this.CanEdit = scratchcard.Status == (int)Enums.ScratchcardStatus.draft;
                this.IdAdminUser = scratchcard.IdAdminUser;
                this.Instructions = scratchcard.Instructions;
                this.GetNotifications = scratchcard.GetNotifications;

                if (scratchcard.Prizes != null)
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
                Type = this.Type,
                DistributionType = this.DistributionType,
                DistributionQuantity = this.DistributionQuantity,
                ScratchcardExpire = this.ScratchcardExpire,
                IdOperation = this.IdOperation,
                Status = this.Status,
                Instructions = this.Instructions,
                GetNotifications = this.GetNotifications
            };
        }

        public StaticText GetRegulation()
        {
            return new StaticText()
            {
                Active = true,
                Created = DateTime.UtcNow,
                Html = this.Regulation,
                IdStaticTextType = (int)Enums.StaticTextType.ScratchcardRegulation,
                Modified = DateTime.UtcNow,
                Title = $"Regulamento - {this.Name}",
                IdOperation = this.IdOperation
            };
        }
    }
}
