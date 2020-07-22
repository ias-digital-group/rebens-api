using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class BannerListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
        public int IdBannerShow { get; set; }
        public bool Active { get; set; }
        public string OperationName { get; set; }
        public string AdminUserCreated { get; set; }
        public string AdminUserModified { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
