using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Parceiro da Operação
    /// </summary>
    public class OperationPartnerModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }
        /// <summary>
        /// Id da Operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// CNPJ
        /// </summary>
        [MaxLength(50)]
        public string Doc { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationPartnerModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationPartnerModel(OperationPartner partner)
        {
            if (partner != null)
            {
                this.Id = partner.Id;
                this.Name = partner.Name;
                this.IdOperation = partner.IdOperation;
                this.Active = partner.Active;
                this.Doc = partner.Doc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OperationPartner GetEntity()
        {
            return new OperationPartner()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Deleted = false,
                Doc = this.Doc
            };
        }
    }

    public class OperationPartnerListItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id da Operação
        /// </summary>
        public int IdOperation { get; set; }
        /// <summary>
        /// Ativo
        /// </summary>
        public bool Active { get; set; }

        public string CreatedUserName { get; set; }
        public string Created { get; set; }
        public string ModifiedUserName { get; set; }
        public string Modified { get; set; }
        public string Doc { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationPartnerListItem() { }

        /// <summary>
        /// Construtor
        /// </summary>
        public OperationPartnerListItem(Entity.OperationPartnerListItem partner)
        {
            if(partner != null)
            {
                this.Id = partner.Id;
                this.Name = partner.Name;
                this.IdOperation = partner.IdOperation;
                this.Active = partner.Active;
                this.Doc = partner.Doc;
                this.CreatedUserName = partner.CreatedUserName;
                this.Created = TimeZoneInfo.ConvertTimeFromUtc(partner.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
                this.ModifiedUserName = partner.ModifiedUserName;
                this.Modified = TimeZoneInfo.ConvertTimeFromUtc(partner.Modified, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider);
            }
        }
    }
}
