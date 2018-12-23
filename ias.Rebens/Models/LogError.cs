using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class LogError
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Complement { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Created { get; set; }
    }
}
