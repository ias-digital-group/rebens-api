using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string OldPassword { get; set; }
        [Required]
        [MaxLength(50)]
        public string NewPassword { get; set; }
        [Required]
        [MaxLength(50)]
        public string NewPasswordConfirm { get; set; }
    }
}
