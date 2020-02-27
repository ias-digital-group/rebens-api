using FluentScheduler;

namespace ias.Rebens.api.helper
{
    /// <summary>
    /// Generate Draw Items
    /// </summary>
    public class GenerateDrawItemsJob : IJob
    {
        private Constant constant;
        public GenerateDrawItemsJob()
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
                if (constant.DebugOn)
                {
                    var log = new LogErrorRepository(constant.ConnectionString);
                    log.Create("GenerateDrawItemsJob", "RUNNED", "", "");
                }
                var repo = new DrawRepository(constant.ConnectionString);
                repo.GenerateItems();
            }
            catch { }
        }
    }
}
