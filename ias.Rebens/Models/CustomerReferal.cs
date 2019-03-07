using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CustomerReferal
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? DegreeOfKinship { get; set; }
        public int IdStatus { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
