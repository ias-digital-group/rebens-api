using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;

namespace ias.Rebens
{
    public class CourseRepository : ICourseRepository
    {
        private string _connectionString;
        public CourseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool AddAddress(int idCourse, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.CourseAddress.Any(o => o.IdCourse == idCourse && o.IdAddress == idAddress))
                    {
                        db.CourseAddress.Add(new CourseAddress() { IdAddress = idAddress, IdCourse = idCourse });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.AddAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool AddPeriod(int idCourse, int idPeriod, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!db.CourseCoursePeriod.Any(o => o.IdCourse == idCourse && o.IdPeriod == idPeriod))
                    {
                        db.CourseCoursePeriod.Add(new CourseCoursePeriod() { IdPeriod = idPeriod, IdCourse = idCourse });
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.AddPeriod", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar adicionar o periodo. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Create(Course course, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    course.Modified = course.Created = DateTime.UtcNow;
                    db.Course.Add(course);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.Create", ex.Message, "", ex.StackTrace);
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
                    var course = db.Course.SingleOrDefault(c => c.Id == id);
                    course.Deleted = true;
                    course.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteAddress(int idCourse, int idAddress, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.CourseAddress.SingleOrDefault(o => o.IdCourse == idCourse && o.IdAddress == idAddress);
                    if (tmp != null)
                    {
                        db.CourseAddress.Remove(tmp);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.DeleteAddress", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeletePeriod(int idCourse, int idPeriod, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmp = db.CourseCoursePeriod.SingleOrDefault(o => o.IdCourse == idCourse && o.IdPeriod == idPeriod);
                    if (tmp != null)
                    {
                        db.CourseCoursePeriod.Remove(tmp);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.DeletePeriod", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o periodo. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Course> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, 
            bool? status = null, int? idCollege = null, int? idAddress = null, List<int> graduationTypes = null, List<int> modalities = null, List<int> periods = null)
        {
            ResultPage<Course> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Course.Where(c => !c.Deleted &&
                                    (!idOperation.HasValue || c.IdOperation == idOperation) &&
                                    (!status.HasValue || c.Active == status) &&
                                    (!idCollege.HasValue || c.IdCollege == idCollege) &&
                                    (!idAddress.HasValue || c.CourseAddresses.Any(a => a.IdAddress == idAddress)) &&
                                    (graduationTypes == null || graduationTypes.Any(t => t == c.IdGraduationType)) &&
                                    (modalities == null || modalities.Any(t => t == c.IdModality)) &&
                                    (periods == null || periods.Any(t => c.CoursePeriods.Any(p => p.IdPeriod == t))) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word)));
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

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Course.Count(c => !c.Deleted &&
                                    (!idOperation.HasValue || c.IdOperation == idOperation) &&
                                    (!status.HasValue || c.Active == status) &&
                                    (!idCollege.HasValue || c.IdCollege == idCollege) &&
                                    (!idAddress.HasValue || c.CourseAddresses.Any(a => a.IdAddress == idAddress)) &&
                                    (graduationTypes == null || graduationTypes.Any(t => t == c.IdGraduationType)) &&
                                    (modalities == null || modalities.Any(t => t == c.IdModality)) &&
                                    (periods == null || periods.Any(t => c.CoursePeriods.Any(p => p.IdPeriod == t))) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word)));

                    ret = new ResultPage<Course>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os cursos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public List<int> ListPeriods(int id, out string error)
        {
            List<int> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.CourseCoursePeriod.Where(p => p.IdCourse == id).Select(p => p.IdPeriod).ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.ListPeriods", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os períodos vinculados ao curso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Course Read(int id, out string error)
        {
            Course ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Course.SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o curso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Course course, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Course.SingleOrDefault(c => c.Id == course.Id);
                    if (update != null)
                    {
                        update.IdCollege = course.IdCollege;
                        update.IdGraduationType = course.IdGraduationType;
                        update.IdModality = course.IdModality;
                        update.Title = course.Title;
                        update.OriginalPrice = course.OriginalPrice;
                        update.Discount = course.Discount;
                        update.FinalPrice = course.FinalPrice;
                        update.Duration = course.Duration;
                        update.Image = course.Image;
                        update.Rating = course.Rating;
                        update.IdAdminUser = course.IdAdminUser;
                        update.DueDate = course.DueDate;
                        update.StartDate = course.StartDate;
                        update.EndDate = course.EndDate;
                        update.VoucherText = course.VoucherText;
                        update.IdDescription = course.IdDescription;
                        update.Active = course.Active;
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
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}
