using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ias.Rebens.Enums;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class LogActionRepository : ILogActionRepository
    {
        private string _connectionString;
        public LogActionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public void Create(Enums.LogAction action, LogItem item, int idItem, int idAdminUser, out string error)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.LogAction.Add(new LogAction() { 
                        Action = (int)action,
                        Created = DateTime.UtcNow,
                        IdItem = (int)idItem,
                        Item = (int)item,
                        IdAdminUser = idAdminUser
                    });
                    db.SaveChanges();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("LogActionRepository.Create", ex.Message, $"idItem: {idItem}, item: {item.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o histórico. (erro:" + idLog + ")";
            }
        }

        public List<LogAction> ListByItem(LogItem item, int idItem, out string error)
        {
            List<LogAction> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.LogAction.Where(a => a.Item == (int)item && a.IdItem == idItem).OrderByDescending(a => a.Created).ToList();
                    
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("LogActionRepository.ListByItem", ex.Message, $"idItem: {idItem}, item: {item.ToString()}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar o histórico. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<LogAction> ListByUser(int idAdminUser, int page, int pageItems, out string error, int? item)
        {
            ResultPage<LogAction> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.LogAction.Where(a => a.IdAdminUser == idAdminUser
                                    && (!item.HasValue || item.Value == a.Item));
                    

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();

                    ret = new ResultPage<LogAction>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("LogActionRepository.ListByUser", ex.Message, $"idAdminUser: {idAdminUser}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as ações. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
