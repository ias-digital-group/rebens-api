using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseView
    {
        public int Id { get; set; }
        public int IdCourse { get; set; }
        public int IdCustomer { get; set; }
        public DateTime Created { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Course Course { get; set; }
    }
}
