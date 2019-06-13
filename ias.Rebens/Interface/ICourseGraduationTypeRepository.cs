using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseGraduationTypeRepository
    {
        CourseGraduationType Read(int id, out string error);

        ResultPage<CourseGraduationType> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        List<CourseGraduationType> ListActive(int idOperation, out string error);

        bool Delete(int id, out string error);

        bool Create(CourseGraduationType graduationType, out string error);

        bool Update(CourseGraduationType graduationType, out string error);
    }
}
