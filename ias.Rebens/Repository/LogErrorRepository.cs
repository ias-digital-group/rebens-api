using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class LogErrorRepository : ILogErrorRepository
    {
        public int Create(LogError log)
        {
            int ret;
            try
            {
                using (var db = new RebensContext())
                {
                    db.LogError.Add(log);
                    db.SaveChanges();
                    ret = log.Id;
                }
            }
            catch
            {
                //Helper.EmailHelper.Send("israel@iasdigitalgroup.com", "Israel", "[vzt] - logerror error", "error on trying to save logError", false);
                ret = 0;
            }
            return ret;
        }
    }
}
