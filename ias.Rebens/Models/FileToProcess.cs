using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class FileToProcess
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int? Total { get; set; }
        public int? Processed { get; set; }
        public int? IdOperation { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
