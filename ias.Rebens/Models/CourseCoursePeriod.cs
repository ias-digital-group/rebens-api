using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseCoursePeriod
    {
        public int IdCourse { get; set; }
        public int IdPeriod { get; set; }

        public virtual Course Course { get; set; }
        public virtual CoursePeriod CoursePeriod { get; set; }
    }
}
