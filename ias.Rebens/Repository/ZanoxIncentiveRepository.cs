using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens
{
    public class ZanoxIncentiveRepository : IZanoxIncentiveRepository
    {
        private string _connectionString;
        public ZanoxIncentiveRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool DisableIncentives(List<int> incentiveIds)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var list = db.ZanoxIncentive.Where(i => !incentiveIds.Any(id => i.Id == id) && i.Active);
                    foreach(var item in list)
                    {
                        item.Modified = DateTime.Now;
                        item.Removed = false;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("ZanoxIncentiveRepository.DisableIncentives", ex.Message, "", ex.StackTrace);
                ret = false;
            }
            return ret;
        }

        public ResultPage<ZanoxIncentive> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<ZanoxIncentive> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.ZanoxIncentive.Include("Program").Where(b => !b.Removed && (string.IsNullOrEmpty(word) || b.Name.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        default:
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<ZanoxIncentive>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxIncentiveRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os incentivos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Save(ZanoxIncentive incentive, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.ZanoxIncentive.SingleOrDefault(i => i.Id == incentive.Id);
                    if(update != null)
                    {
                        update.Active = incentive.Active;
                        update.Amount = incentive.Amount;
                        update.Code = incentive.Code;
                        update.Currency = incentive.Currency;
                        update.CustomerInfo = incentive.CustomerInfo;
                        update.End = incentive.End;
                        update.Modified = DateTime.UtcNow;
                        update.Name = incentive.Name;
                        update.PublisherInfo = incentive.PublisherInfo;
                        update.Removed = false;
                        update.Restriction = incentive.Restriction;
                        update.Start = incentive.Start;
                        update.Type = incentive.Type;
                        update.ZanoxCreated = incentive.ZanoxCreated;
                        update.ZanoxModified = incentive.ZanoxModified;
                    }
                    else
                    {
                        incentive.Created = incentive.Modified = DateTime.UtcNow;
                        db.ZanoxIncentive.Add(incentive);
                    }

                    db.SaveChanges();
                    error = null;

                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                error = $"Ocorreu um erro ao tentar salvar o Incnetivo. (erro: {logError.Create("ZanoxIncentiveRepository.Save", ex.Message, "", ex.StackTrace)})";
                ret = false;
            }
            return ret;
        }

        public void SaveClick(int id, int idCustomer, out string error)
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.ZanoxIncentiveClick.Add(new ZanoxIncentiveClick() { IdZanoxIncentive = id, IdCustomer = idCustomer, Created = DateTime.Now });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("ZanoxIncentiveRepository.SaveClick", ex.Message, $"IdZanoxIncentive: {id}, idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gravar o click no incentivo. (erro:" + idLog + ")";
            }
        }
    }
}
