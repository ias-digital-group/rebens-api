using System;

namespace ias.Rebens.Helper.Config
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public App App { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
    
    public class App
    {
        public string MediaServerPath { get; set; }
        public string MediaVirtualPath { get; set; }
        public string URL { get; set; }
        public string WirecardAuthorization { get; set; }
        public bool Debug { get; set; }

    }
}
