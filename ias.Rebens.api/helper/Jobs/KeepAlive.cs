using FluentScheduler;
using System;
using System.Net;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Keep Alive
    /// </summary>
    public class KeepAlive : IJob
    {
        private Constant constant;
        public KeepAlive()
        {
            this.constant = new Constant();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            if (constant.DebugOn)
            {
                var log = new LogErrorRepository(constant.ConnectionString);
                log.Create("KeepAlive", "RUNNED", "", "");
            }
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri($"{constant.URL}api/portal/homeLocked/"));
                request.Method = "Get";
                request.ContentType = "application/xml";
                request.Headers.Add("x-operation-code", "de6e6ce9-1cf6-46b0-b5ea-2bdc09f359d1");
                var response = request.GetResponse();
            }
            catch { }
        }
    }
}
