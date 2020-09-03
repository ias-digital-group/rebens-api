using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Entity
{
    public class ZanoxProgramListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; }
        public decimal? Rank { get; set; }
        public string Platform { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastIntegrationDate { get; set; }
        public bool Published { get; set; }
    }
}
