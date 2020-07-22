using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class PromoterReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Operation { get; set; }
        public int TotalActive { get; set; }
        public int TotalInactive { get; set; }
        public int TotalValidation { get; set; }
        public int TotalIncomplete { get; set; }
        public int TotalComplete { get; set; }
        public int Total { get { return this.TotalActive + this.TotalInactive; } }
    }
}
