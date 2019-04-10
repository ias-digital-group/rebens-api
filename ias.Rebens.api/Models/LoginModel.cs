using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Login
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }

        /// <summary>
        /// Senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }

    /// <summary>
    /// Validate Model
    /// </summary>
    public class ValidateModel
    {
        /// <summary>
        /// Código de validação
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
        /// <summary>
        /// Confirmação da Senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PasswordConfirm { get; set; }
    }
}
