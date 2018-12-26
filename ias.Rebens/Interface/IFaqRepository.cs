using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFaqRepository
    {
        Faq Read(int id, out string error);

        ResultPage<Faq> ListPage(int page, int pageItems, out string error);

        ResultPage<Faq> SearchPage(string word, int page, int pageItems, out string error);

        bool Delete(int id, out string error);

        bool Create(Faq category, out string error);

        bool Update(Faq category, out string error);

        List<Faq> ListByOperation(int idOperation, out string error);
    }
}
