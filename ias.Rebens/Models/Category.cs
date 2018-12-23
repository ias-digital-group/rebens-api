using System;
using System.Collections.Generic;

namespace ias.Rebens
{
    public partial class Category
    {
        public Category()
        {
            BenefitCategory = new HashSet<BenefitCategory>();
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int? IdParent { get; set; }
        public string Icon { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<BenefitCategory> BenefitCategory { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
