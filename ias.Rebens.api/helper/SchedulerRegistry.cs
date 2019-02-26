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
            Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Minutes();


        }
    }

    public class ZanoxUpdateJob : IJob
    {
        public void Execute()
        {
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
