using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class CategoryItem : Category
    {
        public int? IdBenefit { get; set; }
        public bool HasChild { get; set; }
    }


}
