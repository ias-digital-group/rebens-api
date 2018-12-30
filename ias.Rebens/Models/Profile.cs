using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Profile
    {
        public Profile()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public long Permissions { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }
}
