using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modelo do histórico de pagamento
    /// </summary>
    public class PaymentModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Data de vencimento
        /// </summary>
        public string DueDate { get; set; }
        /// <summary>
        /// Data de pagamento
        /// </summary>
        public string PayDate { get; set; }
        /// <summary>
        /// Número do recibo
        /// </summary>
        public string Receipt { get; set; }
        /// <summary>
        /// Forma de pagamento
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// Valor
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        public PaymentModel() { }

        public PaymentModel(MoipPayment payment)
        {
            this.Id = payment.Id;
            this.DueDate = payment.Created.ToString("dd/MM/yyyy");
            this.PayDate = payment.IdStatus == (int)Enums.MoipPaymentStatus.authorized || payment.IdStatus == (int)Enums.MoipPaymentStatus.done ? payment.Modified.ToString("dd/MM/yyyy") : "";
            this.Receipt = payment.IdMoipInvoice.ToString();
            this.PaymentMethod = string.IsNullOrEmpty(payment.Brand) || string.IsNullOrEmpty(payment.LastFourDigits) ? "" : payment.Brand + " final " + payment.LastFourDigits;
            this.Amount = payment.Amount.ToString("N");
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.MoipPaymentStatus)payment.IdStatus);
        }
    }
}
