using System;

namespace ias.Rebens.Helper
{
    public class LogHelper
    {
        public static int Add(string reference, Exception ex)
        {
            return Add(reference, null, ex);
        }

        public static int Add(string reference, string complement, Exception ex)
        {
            var log = new LogError();
            log.Created = DateTime.Now;
            log.Reference = reference;
            log.Complement = complement;
            log.Message = ex.Message;
            log.StackTrace = ex.StackTrace;

            var logRepo = ServiceLocator<ILogErrorRepository>.Create();
            logRepo.Create(log);

            if (ex.InnerException != null)
            {
                var innerLog = new LogError();

                innerLog.Created = DateTime.Now;
                innerLog.Reference = reference;
                innerLog.Complement = "## INNER " + log.Id + " ##";
                innerLog.Message = ex.InnerException.Message;
                innerLog.StackTrace = ex.InnerException.StackTrace;
                logRepo.Create(innerLog);

                if (ex.InnerException.InnerException != null)
                {
                    var innerLog2 = new LogError();

                    innerLog2.Created = DateTime.Now;
                    innerLog2.Reference = reference;
                    innerLog2.Complement = "## INNER INNER" + innerLog.Id + " ##";
                    innerLog2.Message = ex.InnerException.InnerException.Message;
                    innerLog2.StackTrace = ex.InnerException.InnerException.StackTrace;
                    logRepo.Create(innerLog2);
                }
            }
            return log.Id;
        }

        public static int AddMessage(string reference, string message, string complement)
        {
            var log = new LogError();
            log.Created = DateTime.Now;
            log.Reference = reference;
            log.Complement = "## MESSAGE ##";
            log.Message = message;
            log.StackTrace = complement;

            var logRepo = ServiceLocator<ILogErrorRepository>.Create();
            logRepo.Create(log);

            return log.Id;
        }
    }
}
