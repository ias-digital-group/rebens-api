using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(300)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
