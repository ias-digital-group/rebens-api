using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Token
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Se está autenticado
        /// </summary>
        public bool authenticated { get; set; }
        /// <summary>
        /// Data da criação
        /// </summary>
        public DateTime? created { get; set; }
        /// <summary>
        /// Data que o token irá expirar
        /// </summary>
        public DateTime? expiration { get; set; }
        /// <summary>
        /// Access Token
        /// </summary>
        public string accessToken { get; set; }
    }
}
