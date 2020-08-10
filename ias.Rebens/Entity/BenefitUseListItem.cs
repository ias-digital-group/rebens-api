using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class BenefitUseListItem
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public string BenefitName { get; set; }
        public int IdCustomer { get; set; }
        public int IdBenefit { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCpf { get; set; }
        public DateTime Created { get; set; }
        public DateTime? UsedDate { get; set; }
        public string ApproverName { get; set; }
    }
}
