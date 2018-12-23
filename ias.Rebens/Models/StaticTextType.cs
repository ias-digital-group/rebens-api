using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class StaticTextType
    {
        public StaticTextType()
        {
            StaticText = new HashSet<StaticText>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ICollection<StaticText> StaticText { get; set; }
    }
}
