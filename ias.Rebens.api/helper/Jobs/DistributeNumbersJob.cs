using FluentScheduler;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Generate Numbers Job
    /// </summary>
    public class DistributeNumbersJob : IJob
    {
        private Constant constant;
        public DistributeNumbersJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            try
            {
                var log = new LogErrorRepository(constant.ConnectionString);
                log.Create("DistributeNumbersJob", "START", "", "");
                var repo = new DrawRepository(constant.ConnectionString);
                repo.DistributeNumbers();
                log.Create("DistributeNumbersJob", "END", "", "");
            }
            catch { }
        }
    }
}
