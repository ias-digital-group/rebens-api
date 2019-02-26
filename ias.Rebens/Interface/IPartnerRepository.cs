using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IPartnerRepository
    {
        Partner Read(int id, out string error);

        ResultPage<Partner> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(Partner partner, out string error);

        bool Update(Partner partner, out string error);

        bool AddContact(int idPartner, int idContact, out string error);

        bool AddAddress(int idPartner, int idAddress, out string error);

        bool DeleteContact(int idPartner, int idContact, out string error);

        bool DeleteAddress(int idPartner, int idAddress, out string error);

        bool SetTextId(int id, int idText, out string error);
    }
}
