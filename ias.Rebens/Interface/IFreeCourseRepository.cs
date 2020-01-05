using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFreeCourseRepository
    {
        FreeCourse Read(int id, out string error);
        ResultPage<FreeCourseItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null);
        ResultPage<FreeCourseItem> ListForPortal(int page, int pageItems, string word, string sort, out string error, int idOperation, int? idPartner = null, int ? idCategory = null);
        bool Delete(int id, out string error);
        bool Create(FreeCourse course, out string error);
        bool Update(FreeCourse course, out string error);
        FreeCourse ReadForPortal(int id, out string error);

        bool AddCategory(int idFreeCourse, int idCategory, out string error);
        bool DeleteCategory(int idFreeCourse, int idCategory, out string error);
        ResultPage<FreeCourse> ListByCategory(int idCategory, int page, int pageItems, string word, string sort, out string error);
        bool SaveCategories(int idFreeCourse, string categoryIds, out string error);

        bool ChangeActive(int idFreeCourse, bool active, out string error);

        bool Duplicate(int id, out int newId, out string error);
    }
}
