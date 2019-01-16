using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class EmailModel
    {
        /// <summary>
        /// Email do destinatário
        /// </summary>
        [Required]
        public string FromEmail { get; set; }
        /// <summary>
        /// Nome do destinatário
        /// </summary>
        [Required]
        public string FromName { get; set; }
        /// <summary>
        /// Email do remetente
        /// </summary>
        [Required]
        public string ToEmail { get; set; }
        /// <summary>
        /// Nome do remetente
        /// </summary>
        [Required]
        public string ToName { get; set; }
        /// <summary>
        /// Assunto do e-mail
        /// </summary>
        [Required]
        public string Subject { get; set; }
        /// <summary>
        /// Mensagem
        /// </summary>
        [Required]
        public string Message { get; set; }
    }
}
