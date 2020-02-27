using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Scratchcard
    {
        public Scratchcard()
        {
            Draws = new HashSet<ScratchcardDraw>();
            Prizes = new HashSet<ScratchcardPrize>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? Quantity { get; set; }
        public string NoPrizeImage1 { get; set; }
        public string NoPrizeImage2 { get; set; }
        public string NoPrizeImage3 { get; set; }
        public string NoPrizeImage4 { get; set; }
        public string NoPrizeImage5 { get; set; }
        public string NoPrizeImage6 { get; set; }
        public string NoPrizeImage7 { get; set; }
        public string NoPrizeImage8 { get; set; }
        public int Type { get; set; }
        public int DistributionType { get; set; }
        public int? DistributionQuantity { get; set; }
        public bool ScratchcardExpire { get; set; }
        public int IdOperation { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual ICollection<ScratchcardDraw> Draws { get; set; }
        public virtual ICollection<ScratchcardPrize> Prizes { get; set; }
    }
}
