using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseCollegeAddress
    {
        public int IdCollege { get; set; }
        public int IdAddress { get; set; }

        public virtual Address Address { get; set; }
        public virtual CourseCollege College { get; set; }
    }
}
