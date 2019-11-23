using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class FreeCourseCategory
    {
        public int IdFreeCourse { get; set; }
        public int IdCategory { get; set; }

        public virtual FreeCourse FreeCourse { get; set; }
        public virtual Category Category { get; set; }
    }
}
