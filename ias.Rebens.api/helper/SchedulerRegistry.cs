using System;
using FluentScheduler;

namespace ias.Rebens.api.helper
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            //Schedule<CouponToolsGenerateJob>().ToRunEvery(1).Days().At(0, 10);
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);
            //Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Minutes();


        }
    }

    public class ZanoxUpdateJob : IJob
    {
        IZanoxSaleRepository repo;
        public ZanoxUpdateJob(IZanoxSaleRepository saleRepository) { this.repo = saleRepository; }

        public void Execute()
        {
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while(dt < DateTime.Now)
            {
                var list = zanox.UpdateZanoxSales(DateTime.Now, out string error);
                foreach(var item in list)
                {
                    repo.Save(item, out error);
                }

                dt = dt.AddDays(1);
            }
            
            var mail = new Integration.SendinBlueHelper();
            mail.Send("israel@iasdigitalgroup.com", "Israel", "contato@rebens.com.br", "Rebens", "Teste scheduler", "Run at: " + DateTime.Now.ToString("HH:mm:ss"));
        }
    }

    public class CouponToolsUpdateJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }

    public class CouponToolsGenerateJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
