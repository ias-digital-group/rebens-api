using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// FAQ
    /// </summary>
    public class FaqModel
    {
        /// <summary>
        /// Id da pergunta
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Id da operação
        /// </summary>
        [Required]
        public int IdOperation { get; set; }

        /// <summary>
        /// Pergunta
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Question { get; set; }

        /// <summary>
        /// Resposta
        /// </summary>
        [Required]
        public string Answer { get; set; }

        /// <summary>
        /// Ordem
        /// </summary>
        [Required]
        public int Order { get; set; }

        /// <summary>
        /// Ativo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string StatusName { get { return this.Active ? "Ativo" : "Inativo"; } }
        /// <summary>
        /// Construtor
        /// </summary>
        public FaqModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Faq e popula os atributos
        /// </summary>
        /// <param name="faq"></param>
        public FaqModel(Faq faq) {
            this.Id = faq.Id;
            this.Question = faq.Question;
            this.Answer = faq.Answer;
            this.IdOperation = faq.IdOperation;
            this.Order = faq.Order;
            this.Active = faq.Active;
        }

        /// <summary>
        /// Retorna um objeto Faq com as informações
        /// </summary>
        /// <returns></returns>
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
