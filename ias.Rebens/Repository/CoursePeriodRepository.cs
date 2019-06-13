using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;

namespace ias.Rebens
{
    public class CoursePeriodRepository : ICoursePeriodRepository
    {
        private string _connectionString;
        public CoursePeriodRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CoursePeriod period, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    period.Modified = period.Created = DateTime.UtcNow;
                    db.CoursePeriod.Add(period);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar um período. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var period = db.CoursePeriod.SingleOrDefault(c => c.Id == id);
                    period.Deleted = true;
                    period.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o período. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<CoursePeriod> ListActive(int idOperation, out string error)
        {
            List<CoursePeriod> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CoursePeriod.Where(p => p.IdOperation == idOperation && p.Active && !p.Deleted).OrderBy(p => p.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os períodos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CoursePeriod> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<CoursePeriod> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CoursePeriod.Where(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || idOperation == a.IdOperation));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.CoursePeriod.Count(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || idOperation == a.IdOperation));

                    ret = new ResultPage<CoursePeriod>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os períodos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CoursePeriod Read(int id, out string error)
        {
            CoursePeriod ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CoursePeriod.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o período. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CoursePeriod period, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CoursePeriod.SingleOrDefault(c => c.Id == period.Id && !c.Deleted);
                    if (update != null)
                    {
                        update.Name = period.Name;
                        update.Modified = DateTime.Now;
                        update.Active = period.Active;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Período não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CoursePeriodRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o período. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
