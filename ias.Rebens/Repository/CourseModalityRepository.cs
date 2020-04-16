using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;

namespace ias.Rebens
{
    public class CourseModalityRepository : ICourseModalityRepository
    {
        private string _connectionString;
        public CourseModalityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(CourseModality modality, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    modality.Modified = modality.Created = DateTime.UtcNow;
                    db.CourseModality.Add(modality);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar uma modalidade. (erro:" + idLog + ")";
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
                    var modality = db.CourseModality.SingleOrDefault(c => c.Id == id);
                    modality.Deleted = true;
                    modality.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir a modalidade. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<CourseModality> ListActive(int idOperation, out string error)
        {
            List<CourseModality> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseModality.Where(a => !a.Deleted && a.Active && a.IdOperation == idOperation).OrderBy(a => a.Name).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.ListActive", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as modalidades. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CourseModality> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<CourseModality> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.CourseModality.Where(a => !a.Deleted 
                                    && (string.IsNullOrEmpty(word) || a.Name.Contains(word)) 
                                    && (!idOperation.HasValue || idOperation == a.IdOperation));
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
                    var total = tmpList.Count();

                    ret = new ResultPage<CourseModality>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar as modalidades. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public CourseModality Read(int id, out string error)
        {
            CourseModality ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseModality.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler a modalidade. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(CourseModality modality, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.CourseModality.SingleOrDefault(c => c.Id == modality.Id && !c.Deleted);
                    if (update != null)
                    {
                        update.Modified = DateTime.Now;
                        update.Name = modality.Name;
                        update.Active = modality.Active;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Modalidade não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseModalityRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar a Modalidade. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
