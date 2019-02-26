using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class VoucherModel
    {
        public string ClubLogo { get; set; }
        public string ClubName { get; set; }
        public string PartnerLogo { get; set; }
        public string PartnerName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerDoc { get; set; }
        public string Code { get; set; }
        public string HowToUse { get; set; }
        public string ExpireDate { get; set; }
        public string Discount { get; set; }
    }
}
