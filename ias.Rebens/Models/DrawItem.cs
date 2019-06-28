using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class DrawItem
    {
        public int Id { get; set; }
        public int IdDraw { get; set; }
        public string LuckyNumber { get; set; }
        public int? IdCustomer { get; set; }
        public bool Won { get; set; }
        public int? IdPrize { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Draw Draw { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
