using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IZanoxSaleRepository
    {
        bool Create(ZanoxSale zanoxSale, out string error);
        bool Delete(int id, out string error);
        ResultPage<ZanoxSale> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);
        ResultPage<ZanoxSale> ListPage(int page, int pageItems, string word, string sort, out string error);
        ZanoxSale Read(int id, out string error);
        bool Update(ZanoxSale zanoxSale, out string error);
    }
}
