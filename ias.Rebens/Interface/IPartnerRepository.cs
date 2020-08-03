using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IPartnerRepository
    {
        Partner Read(int id, out string error);

        ResultPage<Entity.PartnerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type = null, bool? status = null);

        bool Delete(int id, int idAdminUser, out string error);

        bool Create(Partner partner, int idAdminUser, out string error);

        bool Update(Partner partner, int idAdminUser, out string error);

        bool AddAddress(int idPartner, int idAddress, out string error);

        bool DeleteAddress(int idPartner, int idAddress, out string error);

        bool SetTextId(int id, int idText, out string error);

        List<Partner> ListFreeCoursePartners(int idOperation, out string error);

        bool ToggleActive(int id, int idAdminUser, out string error);
    }
}
