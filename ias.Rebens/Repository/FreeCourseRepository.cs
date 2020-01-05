using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class FreeCourseRepository : IFreeCourseRepository
    {
        private string _connectionString;
        public FreeCourseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddCategory(int idFreeCourse, int idCategory, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.FreeCourseCategory.Any(o => o.IdFreeCourse == idFreeCourse && o.IdCategory == idCategory))
                    {
                        db.FreeCourseCategory.Add(new FreeCourseCategory() { IdCategory = idCategory, IdFreeCourse = idFreeCourse });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FreeCourseRepository.AddCategory", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar a categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(FreeCourse course, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    course.Modified = course.Created = DateTime.UtcNow;
                    course.Deleted = false;
                    db.FreeCourse.Add(course);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.Create", "", ex);
                error = "Ocorreu um erro ao tentar criar um curso. (erro:" + idLog + ")";
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
                    var course = db.FreeCourse.SingleOrDefault(c => c.Id == id);
                    course.Deleted = true;
                    course.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.Delete", $"id:{id}", ex);
                error = "Ocorreu um erro ao tentar excluir o curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteCategory(int idFreeCourse, int idCategory, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.FreeCourseCategory.SingleOrDefault(o => o.IdFreeCourse == idFreeCourse && o.IdCategory == idCategory);
                    if (tmp != null)
                    {
                        db.FreeCourseCategory.Remove(tmp);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FreeCourseRepository.DeleteCategory", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o categoria. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<FreeCourse> ListByCategory(int idCategory, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<FreeCourse> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.FreeCourse.Where(b => !b.Deleted && !b.Partner.Deleted && b.FreeCourseCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    if (tmpList.Count() < pageItems)
                        page = 0;

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.FreeCourse.Count(b => !b.Deleted && !b.Partner.Deleted && b.FreeCourseCategories.Any(bc => bc.IdCategory == idCategory) && (string.IsNullOrEmpty(word) || b.Title.Contains(word)));

                    ret = new ResultPage<FreeCourse>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FreeCourseRepository.ListByCategory", ex.Message, $"idCategory: {idCategory}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os Cursos Livres. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<FreeCourseItem> ListForPortal(int page, int pageItems, string word, string sort, out string error, int idOperation, int? idPartner = null, int? idCategory = null)
        {
            ResultPage<FreeCourseItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!int.TryParse(word, out int courseId))
                        courseId = 0;
                    var tmpList = db.FreeCourse.Where(c => !c.Deleted && c.IdOperation == idOperation && c.Active &&
                                    (!idPartner.HasValue || c.IdPartner == idPartner) &&
                                    (!idCategory.HasValue || c.FreeCourseCategories.Any(fc => fc.IdCategory == idCategory.Value)) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word) || c.Name.Contains(word) || c.Id == courseId
                                        || c.Description.Contains(word) || c.HowToUse.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        default:
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                    }

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(c => new FreeCourseItem()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Title = c.Title,
                        ListImage = c.ListImage,
                        IdOperation = c.IdOperation,
                        IdAdminUser = c.IdAdminUser,
                        IdPartner = c.IdPartner,
                        Partner = c.Partner,
                        Price = c.Price,
                        Summary = c.Summary,
                        Active = c.Active
                    }).ToList();

                    ret = new ResultPage<FreeCourseItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.ListForPortal", "", ex);
                error = "Ocorreu um erro ao tentar listar os cursos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<FreeCourseItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null)
        {
            ResultPage<FreeCourseItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!int.TryParse(word, out int courseId))
                        courseId = 0;
                    var tmpList = db.FreeCourse.Where(c => !c.Deleted &&
                                    (!idOperation.HasValue || c.IdOperation == idOperation) &&
                                    (!status.HasValue || c.Active == status) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word) || c.Name.Contains(word) || c.Id == courseId
                                        || c.Description.Contains(word) || c.HowToUse.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "title asc":
                            tmpList = tmpList.OrderBy(f => f.Title);
                            break;
                        case "title desc":
                            tmpList = tmpList.OrderByDescending(f => f.Title);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                        default:
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                    }

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).Select(c => new FreeCourseItem() { 
                            Id = c.Id,
                            Name = c.Name,
                            Title = c.Title,
                            ListImage = c.ListImage,
                            IdOperation = c.IdOperation,
                            IdAdminUser = c.IdAdminUser,
                            IdPartner = c.IdPartner,
                            Partner = c.Partner,
                            Price = c.Price,
                            Summary = c.Summary,
                            Active = c.Active
                        }).ToList();

                    ret = new ResultPage<FreeCourseItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.ListPage", "", ex);
                error = "Ocorreu um erro ao tentar listar os cursos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public FreeCourse Read(int id, out string error)
        {
            FreeCourse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FreeCourse.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.Read", $"id: {id}", ex);
                error = "Ocorreu um erro ao tentar ler o curso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public FreeCourse ReadForPortal(int id, out string error)
        {
            FreeCourse ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.FreeCourse.Include("Partner").Include("Partner.StaticText").SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.ReadForPortal", $"id: {id}", ex);
                error = "Ocorreu um erro ao tentar ler o curso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool SaveCategories(int idFreeCourse, string categoryIds, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var categories = db.FreeCourseCategory.Where(b => b.IdFreeCourse == idFreeCourse);
                    db.FreeCourseCategory.RemoveRange(categories);
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(categoryIds))
                    {
                        var ids = categoryIds.Split(',');
                        if (ids.Length > 0)
                        {
                            foreach (var id in ids)
                                db.FreeCourseCategory.Add(new FreeCourseCategory() { IdCategory = int.Parse(id), IdFreeCourse = idFreeCourse });

                            db.SaveChanges();
                        }
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("FreeCourseRepository.SaveCategories", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar salvar as categorias. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(FreeCourse course, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.FreeCourse.SingleOrDefault(c => c.Id == course.Id);
                    if (update != null)
                    {
                        update.IdPartner = course.IdPartner;
                        update.Title = course.Title;
                        update.Name = course.Name;
                        update.Image = course.Image;
                        update.ListImage = course.ListImage;
                        update.IdAdminUser = course.IdAdminUser;
                        update.Price = course.Price;
                        update.Active = course.Active;
                        update.Description = course.Description;
                        update.HowToUse = course.HowToUse;
                        update.Summary = course.Summary;
                        update.Modified = DateTime.UtcNow;
                        

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Curso não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.Update", "", ex);
                error = "Ocorreu um erro ao tentar atualizar o curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool ChangeActive(int idFreeCourse, bool active, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.FreeCourse.SingleOrDefault(c => c.Id == idFreeCourse);
                    if (update != null)
                    {
                        update.Active = active;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                        ret = true;
                    }
                    else
                        error = "Curso não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "FreeCourseRepository.ChangeActive", "", ex);
                error = "Ocorreu um erro ao tentar atualizar o curso. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
