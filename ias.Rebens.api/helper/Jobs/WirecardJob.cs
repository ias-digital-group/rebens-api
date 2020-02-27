using FluentScheduler;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Wirecard Job
    /// </summary>
    public class WirecardJob : IJob
    {
        private Constant constant;
        public WirecardJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var log = new LogErrorRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
            var wireRepo = new WirecardPaymentRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
            var orderRepo = new OrderRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
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
