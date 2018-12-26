using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class OperationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDoc { get; set; }
        public string Image { get; set; }
        public string Domain { get; set; }
        public int? IdContact { get; set; }
        public int IdOperationType { get; set; }
        public decimal? CachbackPercentage { get; set; }
        public bool Active { get; set; }

        public ContactModel Contact { get; set; }

        public OperationModel() { }

        public OperationModel(Operation operation ) {
            this.Id = operation.Id;
            this.Title = operation.Title;
            this.CompanyName = operation.CompanyName;
            this.CompanyDoc = operation.CompanyDoc;
            this.Image = operation.Image;
            this.Domain = operation.Domain;
            this.IdContact = operation.IdContact;
            this.IdOperationType = operation.IdOperationType;
            this.CachbackPercentage = operation.CashbackPercentage;
            this.Active = operation.Active;
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
                IdContact = this.IdContact,
                IdOperationType = this.IdOperationType,
                CashbackPercentage = this.CachbackPercentage,
                Active = this.Active
            };
        }
    }
}
