using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class ZanoxIncentive
    {
        public ZanoxIncentive()
        {
            ZanoxIncentiveClicks = new HashSet<ZanoxIncentiveClick>();
        }

        public int Id { get; set; }
        public int IdProgram { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime? ZanoxCreated { get; set; }
        public DateTime? ZanoxModified { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string PublisherInfo { get; set; }
        public string CustomerInfo { get; set; }
        public string Restriction { get; set; }
        public string Code  { get; set; }
        public string Currency { get; set; }
        public string Url { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Active { get; set; }
        public bool Removed { get; set; }

        public virtual ZanoxProgram Program { get; set; }
        public virtual ICollection<ZanoxIncentiveClick> ZanoxIncentiveClicks { get; set; }
    }
}
