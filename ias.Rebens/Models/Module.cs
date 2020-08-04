using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Information { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Active { get; set; }
    }
}
