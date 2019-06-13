using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseAddress
    {
        public int IdCourse { get; set; }
        public int IdAddress { get; set; }

        public virtual Address Address { get; set; }
        public virtual Course Course { get; set; }
    }
}
