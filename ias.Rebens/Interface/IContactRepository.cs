using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IContactRepository
    {
        Contact Read(int id, out string error);

        ResultPage<Contact> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        bool Delete(int id, out string error);

        bool Create(Contact contact, out string error);

        bool Update(Contact contact, out string error);

        ResultPage<Contact> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Contact> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error);
    }
}
