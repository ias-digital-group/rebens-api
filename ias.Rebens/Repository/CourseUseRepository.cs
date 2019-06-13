using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;

namespace ias.Rebens
{
    public class CourseUseRepository : ICourseUseRepository
    {
        private string _connectionString;
        public CourseUseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CourseUse courseUse, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    while (true)
                    {
                        courseUse.Code = Helper.SecurityHelper.GenerateCode(12);
                        if (db.CourseUse.Any(b => b.Code == courseUse.Code))
                            courseUse.Code = Helper.SecurityHelper.GenerateCode(12);
                        else
                            break;
                    }

                    courseUse.Modified = courseUse.Created = DateTime.UtcNow;
                    db.CourseUse.Add(courseUse);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar um uso. (erro:" + idLog + ")";
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
                    var use = db.CourseUse.SingleOrDefault(c => c.Id == id);
                    db.CourseUse.Remove(use);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o uso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<CourseUse> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<CourseUse> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CourseUse.Where(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word)) && a.IdCustomer == idCustomer);
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
                        case "date asc":
                            tmpList = tmpList.OrderBy(f => f.Created);
                            break;
                        case "date desc":
                            tmpList = tmpList.OrderByDescending(f => f.Created);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.CourseUse.Count(a => (string.IsNullOrEmpty(word) || a.Name.Contains(word)) && a.IdCustomer == idCustomer);

                    ret = new ResultPage<CourseUse>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.ListByCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os usos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CourseUse> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<CourseUse> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CourseUse.Where(a => string.IsNullOrEmpty(word) || a.Name.Contains(word));
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
                    var total = db.CourseUse.Count(a => string.IsNullOrEmpty(word) || a.Name.Contains(word));

                    ret = new ResultPage<CourseUse>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os usos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CourseUse Read(int id, out string error)
        {
            CourseUse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseUse.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o uso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CourseUse ReadByCode(string code, out string error)
        {
            CourseUse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseUse.SingleOrDefault(c => c.Code == code);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.ReadByCode", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o uso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CourseUse courseUse, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CourseUse.SingleOrDefault(c => c.Id == courseUse.Id);
                    if (update != null)
                    {
                        update.OriginalPrice = courseUse.OriginalPrice;
                        update.Discount = courseUse.Discount;
                        update.FinalPrice = courseUse.FinalPrice;
                        update.IdCustomer = courseUse.IdCustomer;
                        update.Modified = DateTime.Now;
                        update.Name = courseUse.Name;
                        update.Status = courseUse.Status;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Uso de Curso não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseUseRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o Uso de curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
