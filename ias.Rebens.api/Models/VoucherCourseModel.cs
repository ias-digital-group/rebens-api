using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class VoucherCourseModel
    {
        public CourseCollege College { get; set; }
        public Customer Customer { get; set; }
        public Course Course { get; set; }
        public Order Order { get; set; }
    }
}
