using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class StaticTextListItem
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
        public int? IdBenefit { get; set; }
        public string CreatedUserName { get; set; }
        public string ModifiedUserName { get; set; }
        public string OperationLogo { get; set; }
        public string OperationName { get; set; }
    }
}
