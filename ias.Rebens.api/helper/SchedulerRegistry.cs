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


            //Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Hours();
            //Schedule<WirecardJob>().ToRunNow().AndEvery(15).Minutes();
            //Schedule<KeepAlive>().ToRunNow().AndEvery(15).Minutes();
            //Schedule<ProcessFileJob>().ToRunNow().AndEvery(5).Minutes();
            //Schedule<ScratchcardJob>().ToRunEvery(1).Days().At(3, 0);
            Schedule(() => new ScratchcardDailyJob(serviceScopeFactory)).ToRunEvery(1).Days().At(1,0);
            Schedule( () => new ScratchcardMonthlyJob(serviceScopeFactory)).ToRunEvery(1).Months().On(1).At(3, 0);
            Schedule(() => new ScratchcardWeeklyJob(serviceScopeFactory)).ToRunEvery(1).Weeks().On(System.DayOfWeek.Sunday).At(4, 0);
        }
    }
}
