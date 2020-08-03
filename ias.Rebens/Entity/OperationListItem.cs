using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class OperationListItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string CreatedUserName { get; set; }
        public string ModifiedUserName { get; set; }
        public string PublishedUserName { get; set; }
        public string Domain { get; set; }
        public string TemporarySubdomain { get; set; }
        public DateTime? TemporaryPublishedDate { get; set; }
    }
}
