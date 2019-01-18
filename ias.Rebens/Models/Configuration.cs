using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public int IdOperation { get; set; }
        public int ConfigurationType { get; set; }
        public string Config { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
