using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ias.Rebens
{
    public class LogErrorRepository : ILogErrorRepository
    {
        private string _connectionString;
        public LogErrorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public LogErrorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Clear()
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
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
                using (var db = new RebensContext(this._connectionString))
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

        public int Create(string reference, string message, string complement, string stackTrace)
        {
            int ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    LogError log = new LogError() {
                        Complement = complement,
                        Created = DateTime.UtcNow,
                        Message = message,
                        Reference = reference,
                        StackTrace = stackTrace
                    };

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

        public int Create(string reference, string complement, Exception ex)
        {
            int ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    LogError log = new LogError()
                    {
                        Complement = complement,
                        Created = DateTime.UtcNow,
                        Message = ex.Message,
                        Reference = reference,
                        StackTrace = ex.StackTrace
                    };
                    db.LogError.Add(log);

                    if (ex.InnerException != null)
                        db.LogError.Add(new LogError()
                        {
                            Complement = complement,
                            Created = DateTime.UtcNow,
                            Message = ex.InnerException.Message,
                            Reference = reference + " - INNER",
                            StackTrace = ex.InnerException.StackTrace
                        });

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
                using (var db = new RebensContext(this._connectionString))
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
                using (var db = new RebensContext(this._connectionString))
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
