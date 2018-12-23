using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class OperationType
    {
        public OperationType()
        {
            Operation = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<Operation> Operation { get; set; }
    }
}
