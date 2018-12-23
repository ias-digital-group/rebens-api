using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class JsonModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Extra { get; set; }
    }
}
