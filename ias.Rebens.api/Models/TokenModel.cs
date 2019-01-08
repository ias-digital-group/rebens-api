using System;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    public class TokenModel
    {
        public bool authenticated { get; set; }
        public DateTime? created { get; set; }
        public DateTime? expiration { get; set; }
        public string accessToken { get; set; }
    }
}
