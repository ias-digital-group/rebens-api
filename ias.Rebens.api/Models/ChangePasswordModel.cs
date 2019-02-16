using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Alteração de senha
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Senha antiga
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string OldPassword { get; set; }

        /// <summary>
        /// Nova senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmação da nova senha
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string NewPasswordConfirm { get; set; }
    }
}
