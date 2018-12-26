using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class FaqModel
    {
        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }

        public FaqModel() { }

        public FaqModel(Faq faq) {
            this.Id = faq.Id;
            this.Question = faq.Question;
            this.Answer = faq.Answer;
            this.IdOperation = faq.IdOperation;
            this.Order = faq.Order;
            this.Active = faq.Active;
        }

        public Faq GetEntity() {
            return new Faq()
            {
                Id = this.Id,
                IdOperation = this.IdOperation,
                Question = this.Question,
                Answer = this.Answer,
                Order = this.Order,
                Active = this.Active
            };
        }
    }
}
