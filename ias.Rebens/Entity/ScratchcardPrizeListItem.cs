using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ScratchcardPrizeListItem
    {
        public int Id { get; set; }
        public string CampaignName { get; set; }
        public int IdScratchcard { get; set; }
        public int IdOperation { get; set; }
        public string OperationName { get; set; }
        public string Prize { get; set; }
        public int Quantity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        
    }
}
