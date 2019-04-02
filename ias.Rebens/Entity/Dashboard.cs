using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class Dashboard
    {
        public DashboardGraph BenefitUse { get; set; }
        public DashboardGraph BenefitView { get; set; }
        public List<DashboardOperation> Operations { get; set; }
    }

    public class DashboardOperation
    {
        public string Operation { get; set; }
        public DashboardGraph Users { get; set; }
        public DashboardGraph RegionState { get; set; }
        public DashboardGraph RegionCity { get; set; }
        public DashboardGraph RegionNeighborhood { get; set; }
        public int TotalReferals { get; set; }
    }

    public class DashboardGraph
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
    }
}
