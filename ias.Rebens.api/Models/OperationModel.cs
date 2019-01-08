using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class OperationModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }
        [Required]
        [MaxLength(300)]
        public string CompanyName { get; set; }
        [MaxLength(50)]
        public string CompanyDoc { get; set; }
        [MaxLength(500)]
        public string Image { get; set; }
        [Required]
        [MaxLength(200)]
        public string Domain { get; set; }
        [Required]
        public int IdOperationType { get; set; }
        public decimal? CachbackPercentage { get; set; }
        [Required]
        public bool Active { get; set; }

        public ContactModel Contact { get; set; }

        public List<OperationContactModel> OperationContacts { get; set; }

        public OperationModel() { }

        public OperationModel(Operation operation) {
            this.Id = operation.Id;
            this.Title = operation.Title;
            this.CompanyName = operation.CompanyName;
            this.CompanyDoc = operation.CompanyDoc;
            this.Image = operation.Image;
            this.Domain = operation.Domain;
            this.IdOperationType = operation.IdOperationType;
            this.CachbackPercentage = operation.CashbackPercentage;
            this.Active = operation.Active;

            if(operation.OperationContacts != null && operation.OperationContacts.Count >0)
            {
                OperationContacts = new List<OperationContactModel>();
                foreach (var item in operation.OperationContacts)
                    OperationContacts.Add(new OperationContactModel() { IdContact = item.IdContact, IdOperation = item.IdOperation });
            }
        }

        public Operation GetEntity()
        {
            return new Operation()
            {
                Id = this.Id,
                Title = this.Title,
                CompanyName = this.CompanyName,
                CompanyDoc = this.CompanyDoc,
                Image = this.Image,
                Domain = this.Domain,
                IdOperationType = this.IdOperationType,
                CashbackPercentage = this.CachbackPercentage,
                Active = this.Active
            };
        }
    }

    public class OperationContactModel
    {
        [Required]
        public int IdOperation { get; set; }
        [Required]
        public int IdContact { get; set; }
    }
}
