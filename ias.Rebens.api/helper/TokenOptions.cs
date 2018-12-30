using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.helper
{
    public class TokenOptions
    {
        public string Type { get; set; } = "Bearer";
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromHours(1);

        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SigningKey { get; set; }
    }
}
