using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Relacionamento Benefício com Operação
    /// </summary>
    public class BenefitOperationModel
    {
        /// <summary>
        /// Id do benefício
        /// </summary>
        [Required]
        public int IdBenefit { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Id da posição
        /// </summary>
        [Required]
        public int IdPosition { get; set; }


        /// <summary>
        /// Construtor
        /// </summary>
        public BenefitOperationModel() { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BenefitOperationItemModel : BenefitOperationModel
    {
        /// <summary>
        /// Nome da operação
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Se a operação está vinculada com o benefício
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Construtor 
        /// </summary>
        public BenefitOperationItemModel()
        {
        }

        /// <summary>
        /// Construtor que já populas as propriedades
        /// </summary>
        /// <param name="item"></param>
        public BenefitOperationItemModel(BenefitOperationItem item)
        {
            this.IdBenefit = item.IdBenefit ?? 0;
            this.IdOperation = item.IdOperation;
            this.IdPosition = item.IdPosition ?? 0;
            this.OperationName = item.OperationName;
            this.Checked = item.IdBenefit.HasValue;
        }
    }
}
