using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ZanoxProgramModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Advertise Rank
        /// </summary>
        public decimal? AdRank { get; set; }
        /// <summary>
        /// Descrição
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Descrição local
        /// </summary>
        public string LocalDescription { get; set; }
        /// <summary>
        /// Início
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Imagem
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Moeda
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Termos
        /// </summary>
        public string Terms { get; set; }
        /// <summary>
        /// Porcentagem máxima de comissão
        /// </summary>
        public decimal? MaxCommissionPercent { get; set; }
        /// <summary>
        /// Porcentagem mínima de comissão
        /// </summary>
        public decimal? MinCommissionPercent { get; set; }
        /// <summary>
        /// Data criaçaõ
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Data última modificação
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// Status (ativo = true, inativo = false)
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Puiblicado 
        /// </summary>
        public bool Published { get; set; }

        public List<ZanoxIncentiveModel> Incentives { get; set; }

        public ZanoxProgramModel() { }
        public ZanoxProgramModel(ZanoxProgram program) 
        {
            if(program != null)
            {
                this.Active = program.Active;
                this.AdRank = program.AdRank;
                this.Created = program.Created;
                this.Currency = program.Currency;
                this.Description = program.Description;
                this.Id = program.Id;
                this.Image = program.Image;
                this.LocalDescription = program.LocalDescription;
                this.MaxCommissionPercent = program.MaxCommissionPercent;
                this.MinCommissionPercent = program.MinCommissionPercent;
                this.Modified = program.Modified;
                this.Name = program.Name;
                this.StartDate = program.StartDate.HasValue ? program.StartDate.Value.ToString("dd/MM/yyyy") : " - ";
                this.Status = program.Status;
                this.Terms = program.Terms;
                this.Url = program.Url;
                this.Published = program.Published;

                if(program.Incentives != null)
                {
                    this.Incentives = new List<ZanoxIncentiveModel>();
                    foreach (var incentive in program.Incentives)
                        this.Incentives.Add(new ZanoxIncentiveModel(incentive));

                }
            }
        }

        public ZanoxProgram GetEntity()
        {
            return new ZanoxProgram()
            {
                Image = this.Image, 
                LocalDescription = this.LocalDescription,
                Modified = DateTime.UtcNow,
                Name = this.Name,
                Published = this.Published,
                Terms = this.Terms,
                Id = this.Id
            };
        }
    }
}
