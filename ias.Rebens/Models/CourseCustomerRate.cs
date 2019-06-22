using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseCustomerRate
    {
        public int IdCourse { get; set; }
        public int IdCustomer { get; set; }
        public int Rate { get; set; }
        public DateTime Created { get; set; }

        public virtual Course Course { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
