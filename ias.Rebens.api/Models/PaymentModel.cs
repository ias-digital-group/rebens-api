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
        public string SignatureCode { get; set; }


        public PaymentModel() { }

        public PaymentModel(MoipPayment payment)
        {
            if (payment != null)
            {
                this.Id = payment.Id;
                this.DueDate = TimeZoneInfo.ConvertTimeFromUtc(payment.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.PayDate = payment.IdStatus == (int)Enums.MoipPaymentStatus.authorized || payment.IdStatus == (int)Enums.MoipPaymentStatus.done ? payment.Modified.ToString("dd/MM/yyyy") : "";
                this.Receipt = payment.IdMoipInvoice.ToString(Constant.FormatProvider);
                this.PaymentMethod = string.IsNullOrEmpty(payment.Brand) || string.IsNullOrEmpty(payment.LastFourDigits) ? "" : payment.Brand + " final " + payment.LastFourDigits;
                this.Amount = payment.Amount.ToString("N", Constant.FormatProvider);
                this.Status = Enums.EnumHelper.GetEnumDescription((Enums.MoipPaymentStatus)payment.IdStatus);
                this.SignatureCode = payment.Signature.Code;
            }
        }
    }
}
