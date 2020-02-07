using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper.Config
{
    public class ModuleInformation
    {
        public List<ModuleFields> Fields { get; set; }
        public List<string> Files { get; set; }
        public ModuleConfiguration Configuration { get; set; }
    }

    public class ModuleFields
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ModuleConfiguration
    {
        public List<string> RouteImports { get; set; }
        public string RouteConfig { get; set; }
    }
}
