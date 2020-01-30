using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper.Config
{
    public class ModuleInformation
    {
        public List<ModuleFields> fields { get; set; }
        public List<string> files { get; set; }
        public ModuleConfiguration configuration { get; set; }
    }

    public class ModuleFields
    {
        public string name { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public List<ModuleFieldsData> data { get; set; }
        public bool isRequired { get; set; }
    }

    public class ModuleFieldsData
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class ModuleConfiguration
    {
        public List<string> routeImports { get; set; }
        public string routeConfig { get; set; }
    }
}
