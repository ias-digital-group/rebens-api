using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class VoucherOrderModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public Operation Operation { get; set; }
    }
}
