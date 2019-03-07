using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Benefício
    /// </summary>
    public class PortalBenefitModel
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
        /// Site do benefício
        /// </summary>
        [MaxLength(500)]
        public string WebSite { get; set; }
        /// <summary>
        /// Chamada do Benefício
        /// </summary>
        public string BenefitCall { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public PortalBenefitModel() { }

        /// <summary>
        /// Construtor que recebe um Benefício e já popula os atributos
        /// </summary>
        /// <param name="benefit"></param>
        public PortalBenefitModel(Benefit benefit)
        {
            this.Id = benefit.Id;
            this.Title = benefit.Title;
            this.WebSite = benefit.Link;
            this.BenefitCall = benefit.Call;

            if (benefit.Partner != null)
                this.Image = benefit.Partner.Logo;
        }
    }
}
