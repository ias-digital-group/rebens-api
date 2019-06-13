using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICoursePeriodRepository
    {
        CoursePeriod Read(int id, out string error);

        ResultPage<CoursePeriod> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        List<CoursePeriod> ListActive(int idOperation, out string error);

        bool Delete(int id, out string error);

        bool Create(CoursePeriod period, out string error);

        bool Update(CoursePeriod period, out string error);
    }
}
