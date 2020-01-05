using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class FreeCourseItem
    {
        public int Id { get; set; }
        public int IdAdminUser { get; set; }
        public int IdPartner { get; set; }
        public int IdOperation { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ListImage { get; set; }
        public decimal Price { get; set; }
        public Partner Partner { get; set; }
        public bool Active { get; set; }
    }
}
