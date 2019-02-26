using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de cupom
    /// </summary>
    public class CouponModel
    {
        /// <summary>
        /// Id 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Campanha
        /// </summary>
        public string Campaign { get; set; }
        /// <summary>
        /// Código
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Data que foi jogado
        /// </summary>
        public string PlayedDate { get; set; }
        /// <summary>
        /// Data de validação
        /// </summary>
        public string ValidationDate { get; set; }
        /// <summary>
        /// Prêmio
        /// </summary>
        public string Prize { get; set; }
        /// <summary>
        /// Data do Cupom
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public CouponModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        public CouponModel(Coupon coupon)
        {
            this.Id = coupon.Id;
            this.Code = coupon.SingleUseCode;
            this.Url = coupon.SingleUseUrl;
            this.PlayedDate = coupon.PlayedDate.HasValue ? coupon.PlayedDate.Value.ToString("dd/MM/yyyy") : "";
            this.ValidationDate = coupon.ValidationDate.HasValue ? coupon.ValidationDate.Value.ToString("dd/MM/yyyy") : "";
            this.Prize = coupon.Value;
            this.Date = coupon.Created.ToString("dd/MM/yyyy");
        }
    }

}
