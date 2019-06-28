using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IDrawRepository
    {
        bool Create(Draw draw, out string error);
        bool Update(Draw draw, out string error);
        void GenerateItems();
        Draw Read(int id, out string error);
        ResultPage<Draw> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);
        bool Delete(int id, out string error);
        ResultPage<DrawItem> ItemListPage(int page, int pageItems, string word, string sort, int idDraw, out string error);
        bool ItemSetCustomer(int idDraw, int idCustomer, out string error);
        bool SetToGenerate(int id, out string error);
    }
}
