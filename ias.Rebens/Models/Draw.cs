using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class Draw
    {
        public Draw()
        {
            DrawItems = new HashSet<DrawItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public int IdOperation { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool Generated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual ICollection<DrawItem> DrawItems { get; set; }

    }
}
