using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class WirecardPaymentModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Id do Pedido
        /// </summary>
        public int IdOrder { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Forma de pagamento
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// linha digitável do boleto
        /// </summary>
        public string BilletLineCode { get; set; }
        /// <summary>
        /// Link para o boleto
        /// </summary>
        public string BilletLink { get; set; }
        /// <summary>
        /// Link para impressão do boleto
        /// </summary>
        public string BilletLinkPrint { get; set; }
        /// <summary>
        /// Bandeira do cartão
        /// </summary>
        public string CardBrand { get; set; }
        /// <summary>
        /// Primeiros 6 digitos do cartão
        /// </summary>
        public string CardFirstSix { get; set; }
        /// <summary>
        /// Últimos 4 digitos do cartão
        /// </summary>
        public string CardLastFour { get; set; }
        /// <summary>
        /// Nome no cartão
        /// </summary>
        public string CardHolderName { get; set; }
        /// <summary>
        /// Data da criação
        /// </summary>
        public DateTime Created { get; set; }

        public WirecardPaymentModel() { }

        public WirecardPaymentModel(WirecardPayment payment) 
        {
            this.Id = payment.Id;
            this.IdOrder = payment.IdOrder;
            this.Status = payment.Status;
            this.Amount = payment.Amount;
            this.Method = payment.Method;
            this.BilletLineCode = payment.BilletLineCode;
            this.BilletLink = payment.BilletLink;
            this.BilletLinkPrint = payment.BilletLinkPrint;
            this.CardBrand = payment.CardBrand;
            this.CardFirstSix = payment.CardFirstSix;
            this.CardLastFour = payment.CardLastFour;
            this.CardHolderName = payment.CardHolderName;
            this.Created = payment.Created;
        }

        public WirecardPayment GetEntity()
        {
            return new WirecardPayment()
            {
                Id = this.Id,
                IdOrder = this.IdOrder,
                Status = this.Status,
                Amount = this.Amount,
                Method = this.Method,
                BilletLineCode = this.BilletLineCode,
                BilletLink = this.BilletLink,
                BilletLinkPrint = this.BilletLinkPrint,
                CardBrand = this.CardBrand,
                CardFirstSix = this.CardFirstSix,
                CardLastFour = this.CardLastFour,
                CardHolderName = this.CardHolderName,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }
    }
}
