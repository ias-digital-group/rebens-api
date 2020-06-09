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
                IWirecardPaymentRepository wireRepo = serviceScope.ServiceProvider.GetService<IWirecardPaymentRepository>();

                log.Create("WirecardSubscriptionJob", "START", "", "");
                wireRepo.ProcessSignatures();
                log.Create("WirecardSubscriptionJob", "END", "", "");
            }
        }
    }
}
