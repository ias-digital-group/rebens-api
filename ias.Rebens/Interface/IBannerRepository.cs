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

        ResultPage<Banner> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Banner> ListByBenefit(int idBenefit, int page, int pageItems, string word, string sort, out string error);

        List<Banner> ListByTypeAndOperation(Guid operationCode, int type, out string error);

        List<Banner> ListByTypeAndOperation(int idOperation, int type, out string error);

        bool AddOperation(int idBanner, int idOperation, out string error);

        bool DeleteOperation(int idBanner, int idOperation, out string error);
    }
}
