using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IStaticTextRepository
    {
        StaticText Read(int id, out string error);

        ResultPage<StaticText> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(StaticText staticText, out string error);

        bool Update(StaticText staticText, out string error);

        StaticText ReadByType(int idOperation, int idType, out string error);
    }
}
