using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICategoryRepository
    {
        CategoryItem Read(int id, out string error);

        ResultPage<Entity.CategoryListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type, bool? status = null, int? idParent = null, int? level = null);

        bool Delete(int id, int idAdminUser, out string error);

        bool Create(Category category, int idAdminUser, out string error);

        bool Update(Category category, int idAdminUser, out string error);

        List<Category> ListTree(int type, bool isCustomer, int? idOperation, out string error);

        List<int> ListByBenefit(int idBenefit, out string error);

        List<int> ListByFreeCourse(int idFreeCourse, out string error);

        List<Category> ListChildren(int idParent, out string error);

        bool ToggleActive(int id, int idAdminUser, out string error);
    }
}
