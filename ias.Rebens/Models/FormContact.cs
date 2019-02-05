using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class FormContact
    {
        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }

        public virtual Operation Operation { get; set; }
    }
}
