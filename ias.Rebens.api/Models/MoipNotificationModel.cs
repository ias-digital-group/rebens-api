using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class MoipNotificationModel
    {
        public string Event { get; set; }
        public string Env { get; set; }
        public DateTime Date { get; set; }
        public object Resource { get; set; }
    }
}
