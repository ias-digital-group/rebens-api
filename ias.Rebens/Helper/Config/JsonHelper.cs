using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper.Config
{
    public static class JsonHelper<T>
    {
        public static T GetObject(string config)
        {
            return JsonConvert.DeserializeObject<T>(config);
        }

        public static string GetString(T config)
        {
            return JsonConvert.SerializeObject(config);
        }
    }
}
