using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CoursePeriod
    {
        public CoursePeriod()
        {
            CoursePeriods = new HashSet<CourseCoursePeriod>();
        }

        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }

        public virtual ICollection<CourseCoursePeriod> CoursePeriods { get; set; }
    }
}
