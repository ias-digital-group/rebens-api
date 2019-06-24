using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper
{
    public static class LogErrorHelper
    {
        public static int Create(string connectionString, string reference, string complement, Exception ex)
        {
            int idLog = 0;
            try
            {
                var logError = new LogErrorRepository(connectionString);
                idLog = logError.Create(reference, ex.Message, complement, ex.StackTrace);
                if (ex.InnerException != null)
                    logError.Create(reference + " - INNER:" + idLog, ex.InnerException.Message, complement, ex.InnerException.StackTrace);
            }
            catch { }
            return idLog;
        }
    }
}
