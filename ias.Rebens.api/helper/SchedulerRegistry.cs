using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry(IServiceScopeFactory serviceScopeFactory)
        {
            //Schedule<GenerateDrawItemsJob>().ToRunNow().AndEvery(15).Minutes();
            //Schedule<DistributeNumbersJob>().ToRunNow().AndEvery(1).Months();
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);
            //Schedule<CouponToolsGenerateJob>().ToRunNow().AndEvery(1).Days().At(0, 30);
            //Schedule(() => new ScratchcardDailyJob(serviceScopeFactory)).ToRunEvery(1).Days().At(1,0);
            //Schedule(() => new ScratchcardMonthlyJob(serviceScopeFactory)).ToRunEvery(1).Months().On(1).At(3, 0);
            //Schedule(() => new ScratchcardWeeklyJob(serviceScopeFactory)).ToRunEvery(1).Weeks().On(System.DayOfWeek.Sunday).At(4, 0);

            using (IServiceScope serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                log.Create("SchedulerRegistry", "START", "", "");
            }

            //Schedule(() => new BenefitLinkCheckerJob(serviceScopeFactory)).ToRunEvery(1).Days().At(2, 0);
            //Schedule(() => new ZanoxUpdateJob(serviceScopeFactory)).ToRunNow().AndEvery(2).Hours();
            //Schedule(() => new WirecardJob(serviceScopeFactory)).ToRunNow().AndEvery(15).Minutes();
            //Schedule(() => new KeepAlive(serviceScopeFactory)).ToRunNow().AndEvery(15).Minutes();
            //Schedule(() => new ProcessFileJob(serviceScopeFactory)).ToRunNow().AndEvery(5).Minutes();
            //Schedule(() => new WirecardSubscriptionJob(serviceScopeFactory)).ToRunNow().AndEvery(10).Minutes();
            Schedule(() => new ZanoxProgramJob(serviceScopeFactory)).ToRunNow();

            //Schedule(() => new CustomerValidationJob(serviceScopeFactory)).ToRunEvery(1).Days().At(23,0);

        }
    }
}
