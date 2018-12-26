using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class LogErrorRepository : ILogErrorRepository
    {
        public bool Clear()
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var logs = db.LogError;
                    db.LogError.RemoveRange(logs);
                    db.SaveChanges();
                }
            }
            catch
            {
                //Helper.EmailHelper.Send("israel@iasdigitalgroup.com", "Israel", "[vzt] - logerror error", "error on trying to save logError", false);
                ret = false;
            }
            return ret;
        }

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

        public bool DeleteOlderThan(DateTime date)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var logs = db.LogError.Where(l => l.Created < date);
                    db.LogError.RemoveRange(logs);
                    db.SaveChanges();
                }
            }
            catch
            {
                //Helper.EmailHelper.Send("israel@iasdigitalgroup.com", "Israel", "[vzt] - logerror error", "error on trying to save logError", false);
                ret = false;
            }
            return ret;
        }

        public ResultPage<LogError> ListPage(int page, int pageItems)
        {
            ResultPage<LogError> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.LogError.OrderByDescending(l => l.Created).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.LogError.Count();

                    ret = new ResultPage<LogError>(list, page, pageItems, total);
                }
            }
            catch
            {
                //Helper.EmailHelper.Send("israel@iasdigitalgroup.com", "Israel", "[vzt] - logerror error", "error on trying to save logError", false);
                ret = null;
            }
            return ret;
        }
    }
}
