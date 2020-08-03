using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class CategoryListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? IdParent { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedUserName { get; set; }
        public int IdType { get; set; }
    }
}
