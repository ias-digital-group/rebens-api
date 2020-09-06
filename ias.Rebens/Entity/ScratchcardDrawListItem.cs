using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ScratchcardDrawListItem
    {
        public int Id { get; set; }
        public string CampaignName { get; set; }
        public int IdScratchcard { get; set; }
        public int IdOperation { get; set; }
        public string OperationName { get; set; }
        public int? IdCustomer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCpf { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime? Date { get; set; }
        public string ValidationCode { get; set; }
        public DateTime? ValidationDate { get; set; }
        public int? IdPrize { get; set; }
        public string Prize { get; set; }

    }
}
