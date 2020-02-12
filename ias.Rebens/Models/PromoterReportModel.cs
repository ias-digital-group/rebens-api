using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class PromoterReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public int TotalActive { get; set; }
        public int TotalInactive { get; set; }
        public int TotalValidation { get; set; }
        public int TotalChangePassword { get; set; }
        public int TotalIncomplete { get; set; }
        public int Total { get { return this.TotalActive + this.TotalChangePassword + this.TotalInactive + this.TotalIncomplete + this.TotalValidation; } }
    }
}
