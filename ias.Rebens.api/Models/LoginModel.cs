﻿using System;
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
}
