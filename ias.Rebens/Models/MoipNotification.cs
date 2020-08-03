using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class MoipNotification
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public string Envoirement { get; set; }
        public string Resources { get; set; }
        public int Status { get; set; }
        public int IdOperation { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
