using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class CourseViewRepository : ICourseViewRepository
    {
        private string _connectionString;
        public CourseViewRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool SaveView(int idCourse, int idCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    db.CourseView.Add(new CourseView() { IdCourse = idCourse, IdCustomer = idCustomer, Created = DateTime.Now });
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("CourseViewRepository.SaveView", ex.Message, $"idCourse: {idCourse}, idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gravar a visualização do curso. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}
