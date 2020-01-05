using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICourseRepository
    {
        Course Read(int id, out string error);
        Course ReadForContract(int id, out string error);
        ResultPage<Course> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null,
            bool? status = null, int? idCollege = null, string address = null, int[] graduationTypes = null, int[] modalities = null, int[] periods = null);
        ResultPage<CourseItem> ListForPortal(int page, int pageItems, string word, string sort, out string error, int idOperation,
            int? idCollege = null, string address = null, List<int> graduationTypes = null, List<int> modalities = null, List<int> periods = null,
            List<string> courseBegin = null, string state = null, string city = null);
        bool Delete(int id, out string error);
        bool Create(Course course, out string error);
        bool Update(Course course, out string error);
        bool AddAddress(int idCourse, int idAddress, out string error);
        bool DeleteAddress(int idCourse, int idAddress, out string error);
        bool AddPeriod(int idCourse, int idPeriod, out string error);
        bool RemovePeriods(int idCourse, out string error);
        bool DeletePeriod(int idCourse, int idPeriod, out string error);
        List<int> ListPeriods(int id, out string error);
        CourseItem ReadForPortal(int id, out string error);
        List<string> ListCourseBegins(int idOperation, out string error);

        List<Tuple<string, string>> ListStates(int idOperation, out string error);
        List<Tuple<string, string>> ListCities(int idOperation, out string error, string state = null);

        bool ChangeActive(int idCourse, bool active, out string error);

        bool Duplicate(int id, out int newId, out string error);
    }
}
