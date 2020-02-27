using System;
using System.Web;
using FluentScheduler;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Zanox Update Job
    /// </summary>
    public class ZanoxUpdateJob : IJob
    {
        private Constant constant;
        public ZanoxUpdateJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var log = new LogErrorRepository(constant.ConnectionString);
            var repo = new ZanoxSaleRepository(constant.ConnectionString);

            log.Create("ZanoxUpdateJob", "START", "", "");
            int counter = 0;
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while (dt < DateTime.Now)
            {
                if (constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), "");
                var list = zanox.UpdateZanoxSales(dt, out string error);
                if (constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), $"total:{list.Count}");
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
