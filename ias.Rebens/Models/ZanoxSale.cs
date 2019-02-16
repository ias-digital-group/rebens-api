using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class ZanoxSale
    {
        public int Id { get; set; }
        public string ZanoxId { get; set; }
        public string ReviewState { get; set; }
        public DateTime? TrackingDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ClickDate { get; set; }
        public long ClickId { get; set; }
        public long ClickInId { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public string Currency { get; set; }
        public int? AdspaceId { get; set; }
        public string AdspaceValue { get; set; }
        public int? AdmediumId { get; set; }
        public string AdmediumValue { get; set; }
        public int? ProgramId { get; set; }
        public string ProgramValue { get; set; }
        public string ReviewNote { get; set; }
        public string Gpps { get; set; }
        public string Zpar { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int IdCustomer { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
