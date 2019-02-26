using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Model de cadastro
    /// </summary>
    public class SignUpModel
    {
        /// <summary>
        /// CPF
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Cpf { get; set; }
        /// <summary>
        /// E-mail
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }
    }
}
