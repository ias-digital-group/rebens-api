using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper.Config
{
    public class OperationConfiguration
    {
        public List<OperationConfigurationField> Fields { get; set; }
        public List<ModuleModel> Modules { get; set; }
        public Wirecard Wirecard { get; set; }
    }

    public class Wirecard
    {
        public string Token { get; set; }
        public string JsToken { get; set; }
    }

    public class OperationConfigurationField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public bool IsRequired { get; set; }
        public List<SelectOptions> Options { get; set; }
    }

    public class SelectOptions
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
