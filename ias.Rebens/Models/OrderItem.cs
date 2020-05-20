using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public int? IdCourse { get; set; }
        public int? IdFreeCourse { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public string Voucher { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Order Order { get; set; }
        public virtual Course Course { get; set; }
        public virtual FreeCourse FreeCourse { get; set; }
    }
}
