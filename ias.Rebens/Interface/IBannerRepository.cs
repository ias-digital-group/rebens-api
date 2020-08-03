using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBannerRepository
    {
        Banner Read(int id, out string error);

        ResultPage<Entity.BannerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, 
                                                    bool? status = null, int? type = null, string where = null);

        bool Delete(int id, int idAdminUser, out string error);

        bool Create(Banner banner, int idAdminUser, out string error);

        bool Update(Banner banner, int idAdminUser, out string error);

        ResultPage<Banner> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Banner> ListByBenefit(int idBenefit, int page, int pageItems, string word, string sort, out string error);

        List<Banner> ListByTypeAndOperation(Guid operationCode, int type, int idBannerShow, out string error);

        List<Banner> ListByTypeAndOperation(int idOperation, int type, int idBannerShow, out string error);

        bool AddOperation(int idBanner, int idOperation, out string error);

        bool DeleteOperation(int idBanner, int idOperation, out string error);

        bool ToggleActive(int id, int idAdminUser, out string error);

        bool ConnectOperations(int id, int[] operations, out string error);
    }
}
