using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseRepository
    {
        Course Read(int id, out string error);

        ResultPage<Course> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? active = null, int? idCollege = null, int? idAddress = null, List<int> graduationTypes = null, List<int> modalities = null, List<int> periods = null);

        bool Delete(int id, out string error);

        bool Create(Course course, out string error);

        bool Update(Course course, out string error);

        bool AddAddress(int idCourse, int idAddress, out string error);

        bool DeleteAddress(int idCourse, int idAddress, out string error);

        bool AddPeriod(int idCourse, int idPeriod, out string error);

        bool DeletePeriod(int idCourse, int idPeriod, out string error);
        List<int> ListPeriods(int id, out string error);
    }
}
