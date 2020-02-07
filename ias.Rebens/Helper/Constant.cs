using System;
using System.IO;
using Newtonsoft.Json;
using ias.Rebens.Helper.Config;


namespace ias.Rebens
{
    public class Constant
    {
        public AppSettings AppSettings { get; set; }
        public Constant() 
        {
            using (StreamReader sr = new StreamReader("appsettings.json"))
            {
                this.AppSettings = JsonConvert.DeserializeObject<AppSettings>(sr.ReadToEnd());
                sr.Close();
            }
        }


        //public static string URL { get { return "https://localhost:44393/"; } } //local
        //public static string ConnectionString { get { return "Server=IAS-02;Database=Rebens;user id=ias_user;password=k4r0l1n4;"; } } //IAS-02
        ////public static string ConnectionString { get { return "Server=SURFACE\\SQLEXPRESS;Database=Rebens;user id=ias_user;password=k4r0l1n4;"; } } //surface
        //public static string MoipNotificationAuthorization { get { return "c7c609fcdb7ef70ac57afdc782574ee3"; } } // Sandbox

        //public static string URL { get { return "http://rebens.iasdigitalgroup.com/"; } } //dev
        //public static string ConnectionString { get { return "Server=172.31.27.205;Database=RebensDev;user id=Rebens_dev;password=#K)YKb4B@&eN;"; } } //dev
        //public static string MoipNotificationAuthorization { get { return "c7c609fcdb7ef70ac57afdc782574ee3"; } } // Sandbox

        //public static string URL { get { return "http://homolog.rebens.com.br/"; } } //Homolog
        //public static string ConnectionString { get { return "Server=172.31.27.205;Database=RebensHomolog;user id=Rebens_dev;password=#K)YKb4B@&eN;"; } } //dev
        //public static string MoipNotificationAuthorization { get { return "fd4be718e70bb84220b1fc3293aba808"; } }

        //public static string URL { get { return "https://admin.rebens.com.br/"; } } // prod
        //public static string ConnectionString { get { return "Server=172.31.27.205;Database=Rebens;user id=Rebens_user;password=4KRe*d9!cd&g;"; } } //prod
        //public static string MoipNotificationAuthorization { get { return "fd4be718e70bb84220b1fc3293aba808"; } }

        public string URL { get { return this.AppSettings.App.URL; } }
        public string ConnectionString { get { return this.AppSettings.ConnectionStrings.DefaultConnection; } }
        public string MoipNotificationAuthorization { get { return this.AppSettings.App.WirecardAuthorization; } }
        public bool DebugOn { get { return this.AppSettings.App.Debug; } }
        public string BuilderUrl { get { return this.AppSettings.App.BuilderUrl; } }
    }
}
