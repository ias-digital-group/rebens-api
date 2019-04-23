using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model Uso Benefício
    /// </summary>
    public class BenefitUseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do Cliente
        /// </summary>
        public int IdCustomer { get; set; }
        /// <summary>
        /// Id do Benefício
        /// </summary>
        public int IdBenefit { get; set; }
        /// <summary>
        /// Nome do Benefício
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id do Typo de benefício
        /// </summary>
        public int IdBenefitType { get; set; }
        /// <summary>
        /// Typo de benefício
        /// </summary>
        public string BenefitType { get { return Enums.EnumHelper.GetEnumDescription((Enums.BenefitType)this.IdBenefitType); } }
        /// <summary>
        /// Valor da compra
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// Comissão
        /// </summary>
        public string Comission { get; set; }
        /// <summary>
        /// Id do Status
        /// </summary>
        public int IdStatus { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Contrutor
        /// </summary>
        public BenefitUseModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="benefitUse"></param>
        public BenefitUseModel(BenefitUse benefitUse)
        {
            this.Id = benefitUse.Id;
            this.IdBenefit = benefitUse.IdBenefit;
            this.IdBenefitType = benefitUse.IdBenefitType;
            this.IdCustomer = benefitUse.IdCustomer;
            this.Name = benefitUse.Name;
            this.Amount = benefitUse.IdBenefitType == (int)Enums.BenefitType.Cashback ? (benefitUse.Amount.HasValue ? benefitUse.Amount.Value.ToString("N") : "") : "";
            this.Comission = benefitUse.IdBenefitType == (int)Enums.BenefitType.Cashback ? (benefitUse.Comission.HasValue ? benefitUse.Comission.Value.ToString("N") : "") : "";
            this.IdStatus = benefitUse.Status;
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.BenefitUseStatus)benefitUse.Status);
            this.Date = benefitUse.Created.ToString("dd/MM/yyyy");
        }
    }
}
