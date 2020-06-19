using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class MoipSignatureModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int IdCustomer { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime NextInvoiceDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public int IdOperation { get; set; }
        public CustomerModel Customer { get; set; }

        public MoipSignatureModel() { }

        public MoipSignatureModel(MoipSignature signature) {
            if (signature != null)
            {
                this.Id = signature.Id;
                this.Code = signature.Code;
                this.IdCustomer = signature.IdCustomer;
                this.PlanCode = signature.PlanCode;
                this.PlanName = signature.PlanName;
                this.CreationDate = TimeZoneInfo.ConvertTimeFromUtc(signature.CreationDate, Constant.TimeZone);
                this.ExpirationDate = TimeZoneInfo.ConvertTimeFromUtc(signature.ExpirationDate, Constant.TimeZone);
                this.NextInvoiceDate = signature.NextInvoiceDate;
                this.PaymentMethod = signature.PaymentMethod;
                this.Status = signature.Status;
                this.Amount = signature.Amount;
                this.IdOperation = signature.IdOperation;
                if (signature.Customer != null)
                    this.Customer = new CustomerModel(signature.Customer);
            }
        }

        public MoipSignature GetMoipSignature()
        {
            return new MoipSignature() {
                Id = this.Id,
                Code = this.Code,
                IdCustomer = this.IdCustomer,
                PlanCode = this.PlanCode,
                PlanName = this.PlanName,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow,
                NextInvoiceDate = this.NextInvoiceDate,
                PaymentMethod = this.PaymentMethod,
                Status = this.Status,
                Amount = this.Amount,
                IdOperation = this.IdOperation,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };
        }

    }
}
