using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper.Config
{
    public class Configuration
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Options { get; set; }
        public bool Required { get; set; }
    }
}
