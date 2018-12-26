using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Faq
    {
        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
