using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class OperationCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string MobilePhone { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Others { get; set; }
        public int IdOperation { get; set; }
        public bool Signed { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
