using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public partial class FreeCourse
    {
        public FreeCourse() 
        { 
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int IdAdminUser { get; set; }
        public int IdPartner { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string HowToUse { get; set; }
        public string Image { get; set; }
        public string ListImage { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual Operation Operation { get; set; }
        public virtual AdminUser AdminUser { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
