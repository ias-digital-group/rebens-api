using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.AddAddress", $"idCourse:{idCourse}, idAddress:{idAddress}", ex);
                error = "Ocorreu um erro ao tentar adicionar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool RemovePeriods(int idCourse, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var periods = db.CourseCoursePeriod.Where(o => o.IdCourse == idCourse);
                    db.CourseCoursePeriod.RemoveRange(periods);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.RemovePeriods", $"idCourse:{idCourse}", ex);
                error = "Ocorreu um erro ao tentar remover os periodos. (erro:" + idLog + ")";
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.AddPeriod", $"idCourse:{idCourse}, idPeriod:{idPeriod}", ex);
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.Create", "", ex);
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.Delete", $"id:{id}", ex);
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.DeleteAddress", $"idCourse:{idCourse}, idAddress:{idAddress}", ex);
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.DeletePeriod", $"idCourse:{idCourse}, idPeriod:{idPeriod}", ex);
                error = "Ocorreu um erro ao tentar excluir o periodo. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Course> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null,
            bool? status = null, int? idCollege = null, string address = null, int[] graduationTypes = null, int[] modalities = null, int[] periods = null)
        {
            ResultPage<Course> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if (!int.TryParse(word, out int courseId))
                        courseId = 0;
                    var tmpList = db.Course.Where(c => !c.Deleted &&
                                    (!idOperation.HasValue || c.IdOperation == idOperation) &&
                                    (!status.HasValue || c.Active == status) &&
                                    (!idCollege.HasValue || c.IdCollege == idCollege) &&
                                    (string.IsNullOrEmpty(address) || c.CourseAddresses.Any(a => a.Address.Street.Contains(address) || a.Address.City.Contains(address))) &&
                                    (graduationTypes == null || graduationTypes.Any(t => t == c.IdGraduationType)) &&
                                    (modalities == null || modalities.Any(t => t == c.IdModality)) &&
                                    (periods == null || periods.Any(t => c.CoursePeriods.Any(p => p.IdPeriod == t))) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word) || c.Id == courseId));
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
                                    (string.IsNullOrEmpty(address) || c.CourseAddresses.Any(a => a.Address.Street.Contains(address) || a.Address.City.Contains(address))) &&
                                    (graduationTypes == null || graduationTypes.Any(t => t == c.IdGraduationType)) &&
                                    (modalities == null || modalities.Any(t => t == c.IdModality)) &&
                                    (periods == null || periods.Any(t => c.CoursePeriods.Any(p => p.IdPeriod == t))) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word) || c.Id == courseId));


                    ret = new ResultPage<Course>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.ListPage", "", ex);
                error = "Ocorreu um erro ao tentar listar os cursos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<CourseItem> ListForPortal(int page, int pageItems, string word, string sort, out string error, int idOperation,
            int? idCollege = null, string address = null, List<int> graduationTypes = null, List<int> modalities = null, List<int> periods = null,
            List<string> courseBegin = null)
        {
            ResultPage<CourseItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Course.Where(c => !c.Deleted && c.Active && c.IdOperation == idOperation &&
                                    (!idCollege.HasValue || c.IdCollege == idCollege) &&
                                    (string.IsNullOrEmpty(address) || c.CourseAddresses.Any(a => a.Address.Street.Contains(address) || a.Address.City.Contains(address))) &&
                                    (graduationTypes == null || graduationTypes.Count == 0 || graduationTypes.Any(t => t == c.IdGraduationType)) &&
                                    (modalities == null || modalities.Count == 0 || modalities.Any(t => t == c.IdModality)) &&
                                    (periods == null || periods.Count == 0 || periods.Any(t => c.CoursePeriods.Any(p => p.IdPeriod == t))) &&
                                    (string.IsNullOrEmpty(word) || c.Title.Contains(word)) &&
                                    (courseBegin == null || courseBegin.Any(cb => c.CourseBegin == cb))).OrderBy(c => c.Title);
                    

                    var total = tmpList.Count();
                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();

                    var resultList = new List<CourseItem>();
                    foreach(var item in list)
                    {
                        var college = db.CourseCollege.Single(c => c.Id == item.IdCollege);
                        var course = new CourseItem(item)
                        {
                            CollegeImage = college.Logo,
                            CollegeName = college.Name
                        };

                        course.GraduationType = db.CourseGraduationType.Single(t => t.Id == item.IdGraduationType).Name;
                        course.Modality = db.CourseModality.Single(m => m.Id == item.IdModality).Name;

                        var periodList = db.CoursePeriod.Where(p => p.CoursePeriods.Any(c => c.IdCourse == item.Id)).Distinct().ToArray();
                        for (int i = 0 ; i < periodList.Count() ; i++)
                        {
                            course.Period += periodList[i].Name;
                            if ((i + 2) < periodList.Count()) course.Period += ", ";
                            else if ((i + 2) == periodList.Count()) course.Period += " ou ";
                        }

                        var addr = db.Address.FirstOrDefault(a => a.CourseCollegeAddresses.Any(c => c.IdCollege == item.IdCollege));
                        if (addr != null)
                            course.Address = addr.City + " - " + addr.State;

                        course.Evaluations = db.CourseCustomerRate.Count(r => r.IdCourse == item.Id);
                        var ratings = db.CourseCustomerRate.Where(r => r.IdCourse == item.Id).Sum(r => (int?)r.Rate) ?? (int)0;
                        if (ratings > 0 && course.Evaluations > 0)
                            course.Rating = (decimal)(ratings / course.Evaluations);

                        resultList.Add(course);
                    }


                    ret = new ResultPage<CourseItem>(resultList, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.ListForPortal", "", ex);
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
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.ListPeriods", $"id: {id}", ex);
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
                    ret = db.Course.Include("CoursePeriods").Include("Description").SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.Read", $"id: {id}", ex);
                error = "Ocorreu um erro ao tentar ler o curso. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Course ReadForContract(int id, out string error)
        {
            Course ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Course.Include("Modality").Include("GraduationType").SingleOrDefault(c => c.Id == id && !c.Deleted);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.Read", $"id: {id}", ex);
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
                        update.Name = course.Name;
                        update.Title = course.Title;
                        update.OriginalPrice = course.OriginalPrice;
                        update.Discount = course.Discount;
                        update.FinalPrice = course.FinalPrice;
                        update.Duration = course.Duration;
                        update.Image = course.Image;
                        update.ListImage = course.ListImage;
                        update.Rating = course.Rating;
                        update.IdAdminUser = course.IdAdminUser;
                        update.DueDate = course.DueDate;
                        update.StartDate = course.StartDate;
                        update.EndDate = course.EndDate;
                        update.VoucherText = course.VoucherText;
                        if (course.IdDescription.HasValue)
                            update.IdDescription = course.IdDescription;
                        else
                            course.IdDescription = update.IdDescription;
                        update.Active = course.Active;
                        update.Modified = DateTime.UtcNow;
                        update.CourseBegin = course.CourseBegin;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Curso não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.Update", "", ex);
                error = "Ocorreu um erro ao tentar atualizar o curso. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool SaveRate(CourseCustomerRate rate, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    if(!db.CourseCustomerRate.Any(c => c.IdCourse == rate.IdCourse && c.IdCustomer == rate.IdCustomer))
                    {
                        rate.Created = DateTime.Now;
                        db.CourseCustomerRate.Add(rate);
                        db.SaveChanges();
                    }
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.SaveRate", "", ex);
                error = "Ocorreu um erro ao tentar salvar a avaliação. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public CourseItem ReadForPortal(int id, out string error)
        {
            CourseItem course;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var item = db.Course.SingleOrDefault(c => c.Id == id);
                    if (item != null)
                    {
                        course = new CourseItem(item);

                        course.GraduationType = db.CourseGraduationType.Single(t => t.Id == item.IdGraduationType).Name;
                        course.Modality = db.CourseModality.Single(m => m.Id == item.IdModality).Name;
                        var college = db.CourseCollege.Single(c => c.Id == item.IdCollege);
                        course.CollegeImage = college.Logo;
                        course.CollegeName = college.Name;

                        var periodList = db.CoursePeriod.Where(p => p.CoursePeriods.Any(c => c.IdCourse == item.Id)).Distinct().ToArray();
                        for (int i = 0; i < periodList.Count(); i++)
                        {
                            course.Period += periodList[i].Name;
                            if ((i + 2) < periodList.Count()) course.Period += ", ";
                            else if ((i + 2) == periodList.Count()) course.Period += " ou ";
                        }

                        var addr = db.Address.FirstOrDefault(a => a.CourseCollegeAddresses.Any(c => c.IdCollege == item.IdCollege));
                        if (addr != null)
                        {
                            course.Address = addr.GetFullAddress();
                            course.AddressShort = addr.GetShortAddress();
                        }

                        course.Evaluations = db.CourseCustomerRate.Count(r => r.IdCourse == item.Id);
                        var ratings = db.CourseCustomerRate.Where(r => r.IdCourse == item.Id).Sum(r => (int?)r.Rate) ?? (int)0;
                        if (ratings > 0 && course.Evaluations > 0)
                            course.Rating = (decimal)(ratings / course.Evaluations);

                        if (item.IdDescription.HasValue)
                        {
                            var desc = db.StaticText.SingleOrDefault(s => s.Id == item.IdDescription.Value);
                            if (desc != null)
                                course.Description = desc.Html;
                        }
                        error = null;
                    }
                    else
                    {
                        error = "Curso não encontrado";
                        course = null;
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.ReadForPortal", $"id:{id}", ex);
                error = "Ocorreu um erro ao tentar ler o curso. (erro:" + idLog + ")";
                course = null;
            }
            return course;
        }

        public List<string> ListCourseBegins(int idOperation, out string error)
        {
            List<string> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Course.Where(c => c.IdOperation == idOperation && c.Active && c.StartDate > DateTime.UtcNow).Select(c => c.CourseBegin).Distinct().ToList();

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogErrorHelper.Create(this._connectionString, "CourseRepository.ListCourseBegins", $"idOperation: {idOperation}", ex);
                error = "Ocorreu um erro ao tentar listar os períodos. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}
