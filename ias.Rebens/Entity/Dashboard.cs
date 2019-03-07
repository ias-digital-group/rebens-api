using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class Dashboard
    {
        public List<DashboardGraph> Benefits { get; set; }
        public List<DashboardUser> Users { get; set; }
    }

    public class DashboardUser
    {
        public string Operation { get; set; }
        public DashboardGraph Users { get; set; }
        public List<DashboardGraph> Region { get; set; }
        public int TotalReferals { get; set; }
    }

    public class DashboardGraph
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public List<DashboardGraphItem> Items { get; set; }
    }

    public class DashboardGraphItem
    {
        public string Title { get; set; }
        public int Total { get; set; }
    }
}
