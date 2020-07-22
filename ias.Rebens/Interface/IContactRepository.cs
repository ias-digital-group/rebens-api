using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IContactRepository
    {
        Contact Read(int id, out string error);

        ResultPage<Entity.ContactListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? type = null, int? idItem = null, bool? active = null, int? idOperation = null);

        bool Delete(int id, int idAdminUSer, out string error);

        bool Create(Contact contact, int idAdminUSer, out string error);

        bool Update(Contact contact, int idAdminUSer, out string error);

        ResultPage<Contact> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Contact> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error);
        bool ToggleActive(int id, int idAdminUser, out string error);
    }
}
