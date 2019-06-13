using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseCollege
    {
        public CourseCollege()
        {
            CollegeAddresses = new HashSet<CourseCollegeAddress>();
            Courses = new HashSet<Course>();
        }

        public int Id { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public int IdAddress { get; set; }
        public string Logo { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<CourseCollegeAddress> CollegeAddresses { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
