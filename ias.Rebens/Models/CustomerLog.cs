using System;

namespace ias.Rebens
{
    public partial class CustomerLog
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int Action { get; set; }
        public string Extra { get; set; }
        public DateTime Created { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
