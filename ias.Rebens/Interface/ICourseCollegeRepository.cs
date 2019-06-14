using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseCollegeRepository
    {
        CourseCollege Read(int id, out string error);

        ResultPage<CourseCollege> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        List<CourseCollege> ListActive(int idOperation, out string error);

        bool Delete(int id, out string error);

        bool Create(CourseCollege college, out string error);

        bool Update(CourseCollege college, out string error);
        bool AddAddress(int id, int idAddress, out string error);
        bool DeleteAddress(int id, int idAddress, out string error);
    }
}
