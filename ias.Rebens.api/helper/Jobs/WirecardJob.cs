using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Wirecard Job
    /// </summary>
    public class WirecardJob : IJob
    {
        private IServiceScopeFactory serviceScopeFactory;

        public WirecardJob(IServiceScopeFactory serviceScopeFactory)
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
                IWirecardPaymentRepository wireRepo = serviceScope.ServiceProvider.GetService<IWirecardPaymentRepository>();
                IOrderRepository orderRepo = serviceScope.ServiceProvider.GetService<IOrderRepository>();
                
                log.Create("WirecardJob", "START", "", "");
                if (wireRepo.HasPaymentToProcess())
                {
                    log.Create("WirecardJob.Payments", "START", "", "");
                    wireRepo.ProcessPayments();
                    log.Create("WirecardJob.Payments", "FINISH", "", "");
                }
                else
                    log.Create("WirecardJob.Payments", "NO-PAYMENTS", "", "");

                if (orderRepo.HasOrderToProcess())
                {
                    log.Create("WirecardJob.Orders", "START", "", "");
                    orderRepo.ProcessOrder();
                    log.Create("WirecardJob.Orders", "FINISH", "", "");
                }
                else
                    log.Create("WirecardJob.Orders", "NO-ORDERS", "", "");
            }
        }
    }
}
