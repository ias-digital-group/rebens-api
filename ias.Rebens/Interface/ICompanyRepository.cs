using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICompanyRepository
    {
        Company Read(int id, out string error);
        bool Create(Company company, int idAdminUser, out string error);
        bool Update(Company company, int idAdminUser, out string error);
        bool Delete(int id, int idAdminUser, out string error);
        bool ToggleActive(int id, int idAdminUser, out string error);
        ResultPage<CompanyItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type = null, int? idItem = null, bool? status = null);
    }
}
