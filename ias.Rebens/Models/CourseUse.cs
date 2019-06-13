using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class CourseUse
    {
        public int Id { get; set; }
        public int IdCourse { get; set; }
        public int IdCustomer { get; set; }
        public string Name { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Course Course { get; set; }
    }
}
