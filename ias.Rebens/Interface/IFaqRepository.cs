using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IFaqRepository
    {
        Faq Read(int id, out string error);

        ResultPage<Faq> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);

        bool Delete(int id, out string error);

        bool Create(Faq category, out string error);

        bool Update(Faq category, out string error);

        ResultPage<Faq> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        List<Faq> ListByOperation(Guid operationCode, out string error);
    }
}
