using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class ScratchcardPrize
    {
        public ScratchcardPrize()
        {
            Draws = new HashSet<ScratchcardDraw>();
        }

        public int Id { get; set; }
        public int IdScratchcard { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Odds { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public Scratchcard Scratchcard { get; set; }
        public ICollection<ScratchcardDraw> Draws { get; set; }
    }
}
