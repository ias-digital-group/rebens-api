using System;
using System.ComponentModel.DataAnnotations;


namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Formulário de Contato
    /// </summary>
    public class FormContactModel
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
        /// Email
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Email { get; set; }
        /// <summary>
        /// Telefone
        /// </summary>
        [MaxLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// Mensagem
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public FormContactModel() { }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="contact"></param>
        public FormContactModel(FormContact contact) {
            this.Id = contact.Id;
            this.Name = contact.Name;
            this.Email = contact.Email;
            this.Phone = contact.Phone;
            this.Message = contact.Message;
        }

        /// <summary>
        /// Retorna um contato
        /// </summary>
        /// <returns></returns>
        public FormContact GetEntity()
        {
            var contact = new FormContact()
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Phone = this.Phone,
                Message = this.Message,
                Created = DateTime.Now
            };
            if(!string.IsNullOrEmpty(contact.Phone))
            {
                if(contact.Phone.Length == 10)
                    contact.Phone = "(" + contact.Phone.Substring(0, 2) + ") " + contact.Phone.Substring(2, 4) + "-" + contact.Phone.Substring(6);
                else if(contact.Phone.Length == 11)
                    contact.Phone = "(" + contact.Phone.Substring(0, 2) + ") " + contact.Phone.Substring(2, 5) + "-" + contact.Phone.Substring(7);
            }

            return contact;
        }
    }
}
