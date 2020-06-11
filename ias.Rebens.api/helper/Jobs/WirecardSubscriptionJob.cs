using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    public class WirecardSubscriptionJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public WirecardSubscriptionJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Execute()
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                ILogErrorRepository log = serviceScope.ServiceProvider.GetService<ILogErrorRepository>();
                IMoipNotificationRepository repo = serviceScope.ServiceProvider.GetService<IMoipNotificationRepository>();

                log.Create("WirecardSubscriptionJob", "START", "", "");
                if (repo.HasSubscriptionToProcess())
                {
                    log.Create("WirecardSubscriptionJob.ProcessSubscription", "START", "", "");
                    repo.ProcessSubscription();
                    log.Create("WirecardSubscriptionJob.ProcessSubscription", "DONE", "", "");
                }

                if (repo.HasInvoicesToProcess())
                {
                    log.Create("WirecardSubscriptionJob.ProcessInvoices", "START", "", "");
                    repo.ProcessInvoices();
                    log.Create("WirecardSubscriptionJob.ProcessInvoices", "DONE", "", "");
                }

                if (repo.HasPaymentsToProcess())
                {
                    log.Create("WirecardSubscriptionJob.ProcessPayments", "START", "", "");
                    repo.ProcessPayments();
                    log.Create("WirecardSubscriptionJob.ProcessPayments", "DONE", "", "");
                }

                //wireRepo.ProcessSignatures();
                log.Create("WirecardSubscriptionJob", "END", "", "");
            }
        }
    }
}
