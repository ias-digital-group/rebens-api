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

        public OperationPartnerModel(OperationPartner partner)
        {
            this.Id = partner.Id;
            this.Name = partner.Name;
            this.IdOperation = partner.IdOperation;
            this.Active = partner.Active;
        }

        public OperationPartner GetEntity()
        {
            return new OperationPartner()
            {
                Id = this.Id,
                Name = this.Name,
                IdOperation = this.IdOperation,
                Active = this.Active,
                Deleted = false
            };
        }
    }
}
