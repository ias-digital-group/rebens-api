using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class FaqModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int IdOperation { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Question { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Answer { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
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
