using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class PartnerContact
    {
        public int IdPartner { get; set; }
        public int IdContact { get; set; }

        public virtual Contact Contacts { get; set; }
        public virtual Partner Partners { get; set; }
    }
}
