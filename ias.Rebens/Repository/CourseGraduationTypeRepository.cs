using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;

namespace ias.Rebens
{
    public class CourseGraduationTypeRepository : ICourseGraduationTypeRepository
    {
        private string _connectionString;
        public CourseGraduationTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CourseGraduationType graduationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    graduationType.Modified = graduationType.Created = DateTime.UtcNow;
                    db.CourseGraduationType.Add(graduationType);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar um tipo de graduação. (erro:" + idLog + ")";
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
                    var graduationType = db.CourseGraduationType.SingleOrDefault(c => c.Id == id);
                    graduationType.Deleted = true;
                    graduationType.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o tipo de graduação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<CourseGraduationType> ListActive(int idOperation, out string error)
        {
            List<CourseGraduationType> ret;
            try
            {
                using(var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseGraduationType.Where(t => t.Active && !t.Deleted && t.IdOperation == idOperation).OrderBy(t => t.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de graduação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CourseGraduationType> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<CourseGraduationType> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CourseGraduationType.Where(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || a.IdOperation == idOperation));
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
                    var total = db.CourseGraduationType.Count(a => !a.Deleted && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || !idOperation.HasValue || a.IdOperation == idOperation));

                    ret = new ResultPage<CourseGraduationType>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os tipos de graduação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CourseGraduationType Read(int id, out string error)
        {
            CourseGraduationType ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseGraduationType.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o tipo de graduação. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CourseGraduationType graduationType, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CourseGraduationType.SingleOrDefault(c => c.Id == graduationType.Id);
                    if (update != null)
                    {
                        update.Modified = DateTime.UtcNow;
                        update.Name = graduationType.Name;
                        update.Active = graduationType.Active;
                        update.Parent = graduationType.Parent;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Tipo de graduação não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseGraduationTypeRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o tipo de graduação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
