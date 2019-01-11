using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Login 
    /// </summary>
    public class LoginResultModel
    {
        /// <summary>
        /// Token de acesso
        /// </summary>
        public TokenModel Token { get; set; }
        /// <summary>
        /// Usuário
        /// </summary>
        public UserModel User { get; set; }
        /// <summary>
        /// Papel
        /// </summary>
        public string Role { get; set; }
    }

    /// <summary>
    /// Usuário
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
    }
}
