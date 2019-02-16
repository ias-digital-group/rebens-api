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
    }
}
