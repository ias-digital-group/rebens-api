using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBannerRepository
    {
        Banner Read(int id, out string error);

        ResultPage<Banner> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(Banner banner, out string error);

        bool Update(Banner banner, out string error);

        List<Banner> ListByOperation(int idOperation, out string error);

        List<Banner> ListByBenefit(int idBenefit, out string error);
    }
}
