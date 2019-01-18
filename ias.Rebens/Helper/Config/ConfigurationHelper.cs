using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ias.Rebens.Helper.Config
{
    public static class ConfigurationHelper
    {
        public static List<Configuration> GetConfigurations(string config)
        {
            return JsonConvert.DeserializeObject<List<Configuration>>(config);
        }

        public static string GetConfigurationString(List<Configuration> configurations)
        {
            return JsonConvert.SerializeObject(configurations);
        }

        public static List<ConfigurationValue> GetConfigurationValues(string config)
        {
            return JsonConvert.DeserializeObject<List<ConfigurationValue>>(config);
        }

        public static string GetConfigurationValueString(List<ConfigurationValue> config)
        {
            return JsonConvert.SerializeObject(config);
        }
    }
}
