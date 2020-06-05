using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBenefitUseRepository
    {
        bool Create(BenefitUse benefitUse, out string error);
        bool Delete(int id, out string error);
        ResultPage<BenefitUse> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);
        ResultPage<BenefitUse> ListPage(int page, int pageItems, string word, string sort, out string error);
        BenefitUse Read(int id, out string error);
        bool Update(BenefitUse benefitUse, out string error);
        BenefitUse ReadByCode(string code, out string error);
        bool GetCustomerWithdrawSummary(int idCustomer, out decimal available, out decimal blocked, out string error);
        decimal GetCustomerBalance(int idCustomer, out string error);
        ResultPage<Entity.BenefitUseListItem> ValidateListPage(int page, int pageItems, string word, out string error, int? idPartner = null);

    }
}
