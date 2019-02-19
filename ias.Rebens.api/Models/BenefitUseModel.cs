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
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id do Cliente
        /// </summary>
        [Required]
        public int IdCustomer { get; set; }
        /// <summary>
        /// Id do Benefício
        /// </summary>
        [Required]
        public int IdBenefit { get; set; }
        /// <summary>
        /// Nome do Benefício
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Id do Typo de benefício
        /// </summary>
        [Required]
        public int IdBenefitType { get; set; }
        /// <summary>
        /// Typo de benefício
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string BenefitType { get; set; }
        /// <summary>
        /// Valor da compra
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Comissão
        /// </summary>
        public decimal? Comission { get; set; }
        /// <summary>
        /// Id do Status
        /// </summary>
        [Required]
        public int IdStatus { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        [Required]
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
            this.Amount = benefitUse.Amount;
            this.Comission = benefitUse.Comission;
            this.IdStatus = benefitUse.Status;
            this.Status = Enums.EnumHelper.GetEnumDescription((Enums.BenefitUseStatus)benefitUse.Status);
            this.Date = benefitUse.Created.ToString("dd/MM/yyyy");
            if (benefitUse.BenefitType != null)
                this.BenefitType = benefitUse.BenefitType.Name;
        }

        /// <summary>
        /// Get the object
        /// </summary>
        /// <returns></returns>
        public BenefitUse GetEntity()
        {
            return new BenefitUse()
            {
                Id = this.Id,
                IdBenefit = this.IdBenefit,
                IdCustomer = this.IdCustomer,
                IdBenefitType = this.IdBenefitType,
                Name = this.Name,
                Amount = this.Amount,
                Comission = this.Comission,
                Status = this.IdStatus,
                Modified = DateTime.Now
            };
        }
    }
}
