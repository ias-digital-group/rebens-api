using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Keep Alive
    /// </summary>
    public class KeepAlive : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public KeepAlive(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                log.Create("KeepAlive", "RUNNED", "", "");
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri($"{new Constant().URL}api/portal/homeLocked/"));
                    request.Method = "Get";
                    request.ContentType = "application/xml";
                    request.Headers.Add("x-operation-code", "de6e6ce9-1cf6-46b0-b5ea-2bdc09f359d1");
                    var response = request.GetResponse();
                }
                catch { }
            }
        }
    }
}
