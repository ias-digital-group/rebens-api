using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BannerOperation
    {
        public int IdBanner { get; set; }
        public int IdOperation { get; set; }

        public virtual Banner IdBannerNavigation { get; set; }
        public virtual Operation IdOperationNavigation { get; set; }
    }
}
