using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICategoryRepository
    {
        Category Read(int id, out string error);

        ResultPage<Category> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(Category category, out string error);

        bool Update(Category category, out string error);

        List<Category> ListTree(bool isCustomer, int? idOperation, out string error);

        List<int> ListByBenefit(int idBenefit, out string error);

        List<Category> ListChildren(int idParent, out string error);
    }
}
