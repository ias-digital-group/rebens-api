using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ContactListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ComercialPhone { get; set; }
        public string ComercialPhoneBranch { get; set; }
        public string Cellphone { get; set; }
        public bool Active { get; set; }
        public string RelationshipName { get; set; }
        public int? Type { get; set; }
        public int? IdItem { get; set; }
    }
}
