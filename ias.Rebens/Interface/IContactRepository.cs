using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IContactRepository
    {
        Contact Read(int id, out string error);

        ResultPage<Contact> ListPage(int page, int pageItems, out string error);

        ResultPage<Contact> SearchPage(string word, int page, int pageItems, out string error);

        bool Delete(int id, out string error);

        bool Create(Contact contact, out string error);

        bool Update(Contact contact, out string error);
    }
}
