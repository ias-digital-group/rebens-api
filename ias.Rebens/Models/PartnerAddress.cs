using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class PartnerAddress
    {
        public int IdPartner { get; set; }
        public int IdAddress { get; set; }

        public virtual Address Addresses { get; set; }
        public virtual Partner Partners { get; set; }
    }
}
