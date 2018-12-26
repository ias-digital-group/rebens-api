using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class BannerOperation
    {
        public int IdBanner { get; set; }
        public int IdOperation { get; set; }

        public virtual Banner Banner { get; set; }
        public virtual Operation Operation { get; set; }
    }
}
