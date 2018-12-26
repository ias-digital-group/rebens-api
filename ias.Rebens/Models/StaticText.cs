using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class StaticText
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public string Style { get; set; }
        public int Order { get; set; }
        public int IdStaticTextType { get; set; }
        public int IdOperation { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual StaticTextType StaticTextType { get; set; }
    }
}
