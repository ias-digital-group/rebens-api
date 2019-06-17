using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class CourseCollegeRepository : ICourseCollegeRepository
    {
        private string _connectionString;
        public CourseCollegeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int id, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.CourseCollegeAddress.Any(o => o.IdCollege == id && o.IdAddress == idAddress))
                    {
                        db.CourseCollegeAddress.Add(new CourseCollegeAddress() { IdAddress = idAddress, IdCollege = id });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(CourseCollege college, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    college.Modified = college.Created = DateTime.UtcNow;
                    db.CourseCollege.Add(college);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar uma faculdade. (erro:" + idLog + ")";
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
                    var college = db.CourseCollege.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    college.Deleted = true;
                    college.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a faculdade. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int id, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.CourseCollegeAddress.SingleOrDefault(o => o.IdCollege == id && o.IdAddress == idAddress);
                    db.CourseCollegeAddress.Remove(tmp);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.DeleteAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<CourseCollege> ListActive(int idOperation, out string error)
        {
            List<CourseCollege> ret;
            try
            {
                using(var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseCollege.Where(c => c.Active && !c.Deleted && c.IdOperation == idOperation).OrderBy(c => c.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as faculdades. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CourseCollege> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<CourseCollege> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CourseCollege.Where(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || a.IdOperation == idOperation));
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
                    var total = db.CourseCollege.Count(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || a.IdOperation == idOperation));

                    ret = new ResultPage<CourseCollege>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as faculdades. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CourseCollege Read(int id, out string error)
        {
            CourseCollege ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseCollege.Include("Address").SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a faculdade. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CourseCollege college, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CourseCollege.SingleOrDefault(c => c.Id == college.Id && !c.Deleted);
                    if (update != null)
                    {
                        update.Name = college.Name;
                        update.Logo = college.Logo;
                        update.Active = college.Active;
                        update.Modified = DateTime.Now;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Faculdade não encontrada!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseCollegeRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a faculdade. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
