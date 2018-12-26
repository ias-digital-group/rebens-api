using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class AdminUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }
        public string EncryptedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public int IdProfile { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
