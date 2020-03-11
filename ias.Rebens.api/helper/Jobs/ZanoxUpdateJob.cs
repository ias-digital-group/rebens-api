using System;
using System.Web;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Zanox Update Job
    /// </summary>
    public class ZanoxUpdateJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public ZanoxUpdateJob(IServiceScopeFactory serviceScopeFactory)
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
                IZanoxSaleRepository repo = serviceScope.ServiceProvider.GetService<IZanoxSaleRepository>();

                log.Create("ZanoxUpdateJob", "START", "", "");
                int counter = 0;
                var zanox = new Integration.ZanoxHelper();
                var dt = DateTime.Now.AddMonths(-1);
                while (dt < DateTime.Now)
                {
                    log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), "");
                    var list = zanox.UpdateZanoxSales(dt, out string error);
                    log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), $"total:{list.Count}");
                    counter += list.Count;
                    foreach (var item in list)
                    {
                        item.Zpar = string.IsNullOrEmpty(item.Zpar) ? "" : HttpUtility.UrlDecode(item.Zpar);
                        repo.Save(item, out error);
                    }

                    dt = dt.AddDays(1);
                }
                log.Create("ZanoxUpdateJob", "FINISH", "", "");
            }
        }
    }
}
