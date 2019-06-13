using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseUseRepository
    {
        bool Create(CourseUse courseUse, out string error);
        bool Delete(int id, out string error);
        ResultPage<CourseUse> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);
        ResultPage<CourseUse> ListPage(int page, int pageItems, string word, string sort, out string error);
        CourseUse Read(int id, out string error);
        CourseUse ReadByCode(string code, out string error);
        bool Update(CourseUse courseUse, out string error);
    }
}
