using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class ScratchcardDraw
    {
        public int Id { get; set; }
        public int IdScratchcard { get; set; }
        public int? IdScratchcardPrize { get; set; }
        public string Image { get; set; }
        public int? IdCustomer { get; set; }
        public string Prize { get; set; }
        public string ValidationCode { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? PlayedDate { get; set; }
        public DateTime? ValidationDate { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Scratchcard Scratchcard { get; set; }
        public virtual ScratchcardPrize ScratchcardPrize { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
