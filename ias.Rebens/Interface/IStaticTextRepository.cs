using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IStaticTextRepository
    {
        StaticText Read(int id, out string error);

        StaticText ReadText(int idOperation, string page, out string error);

        ResultPage<Entity.StaticTextListItem> ListPage(int page, int pageItems, string word, string sort, int idStaticTextType, out string error, int? idOperation = null);

        bool Delete(int id, out string error);

        bool Create(StaticText staticText, out string error);

        bool Update(StaticText staticText, int idAdminUser, out string error);

        StaticText ReadByType(int idOperation, int idType, out string error);

        StaticText ReadByType(int idType, out string error);

        StaticText ReadText(Guid operationCode, string page, out string error);

        ResultPage<StaticText> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        StaticText ReadOperationConfiguration(int idOperation, out string error);

        List<StaticText> ListByType(int idStaticTextType, out string error, int? idOperation = null);
    }
}
