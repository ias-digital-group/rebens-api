using Amazon.S3.Model.Internal.MarshallTransformations;
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
        public string CreationDateString { get { return this.CreationDate.ToString("dd/MM/yyyy HH:mm", Constant.FormatProvider); } }
        public DateTime ExpirationDate { get; set; }
        public string ExpirationDateString { get { return this.ExpirationDate.ToString("dd/MM/yyyy HH:mm", Constant.FormatProvider); } }
        public DateTime NextInvoiceDate { get; set; }
        public string NextInvoiceDateString { get { return this.NextInvoiceDate.ToString("dd/MM/yyyy", Constant.FormatProvider); } }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string AmountString { get { return this.Amount.ToString("N", Constant.FormatProvider); } }
        public int IdOperation { get; set; }
        public string OperationName { get; set; }
        public CustomerModel Customer { get; set; }
        public string Modified { get; set; }

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
                this.NextInvoiceDate = TimeZoneInfo.ConvertTimeFromUtc(signature.NextInvoiceDate, Constant.TimeZone);
                this.PaymentMethod = signature.PaymentMethod;
                switch (signature.Status.ToUpper())
                {
                    case "ACTIVE":
                        this.Status = "Ativa";
                        break;
                    case "SUSPENDED":
                        this.Status = "Suspensa";
                        break;
                    case "EXPIRED":
                        this.Status = "Expirada";
                        break;
                    case "OVERDUE":
                        this.Status = "Atrasada";
                        break;
                    case "CANCELED":
                        this.Status = "Cancelada";
                        break;
                    case "TRIAL":
                        this.Status = "Teste";
                        break;
                }
                
                this.Amount = signature.Amount;
                this.IdOperation = signature.IdOperation;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(signature.Modified, Constant.TimeZone).ToString("dd/MM/yyyy HH:mm", Constant.FormatProvider);
                if (signature.Customer != null)
                    this.Customer = new CustomerModel(signature.Customer);
            }
        }

        public MoipSignatureModel(Entity.MoipSignatureItem signature)
        {
            if (signature != null)
            {
                this.Id = signature.Id;
                this.Code = signature.Code;
                this.IdCustomer = signature.IdCustomer;
                this.PlanCode = signature.PlanCode;
                this.PlanName = signature.PlanName;
                this.CreationDate = TimeZoneInfo.ConvertTimeFromUtc(signature.CreationDate, Constant.TimeZone);
                this.NextInvoiceDate = TimeZoneInfo.ConvertTimeFromUtc(signature.NextInvoiceDate, Constant.TimeZone);
                this.PaymentMethod = signature.PaymentMethod;
                switch (signature.Status.ToUpper())
                {
                    case "ACTIVE":
                        this.Status = "Ativa";
                        break;
                    case "SUSPENDED":
                        this.Status = "Suspensa";
                        break;
                    case "EXPIRED":
                        this.Status = "Expirada";
                        break;
                    case "OVERDUE":
                        this.Status = "Atrasada";
                        break;
                    case "CANCELED":
                        this.Status = "Cancelada";
                        break;
                    case "TRIAL":
                        this.Status = "Teste";
                        break;
                }
                this.OperationName = signature.OperationName;
                this.Amount = signature.Amount;
                this.IdOperation = signature.IdOperation;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(signature.Modified, Constant.TimeZone).ToString("dd/MM/yyyy HH:mm", Constant.FormatProvider);
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
