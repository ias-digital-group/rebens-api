using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseModalityRepository
    {
        CourseModality Read(int id, out string error);

        ResultPage<CourseModality> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        List<CourseModality> ListActive(int idOperation, out string error);

        bool Delete(int id, out string error);

        bool Create(CourseModality modality, out string error);

        bool Update(CourseModality modality, out string error);
    }
}
