using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IWithdrawRepository
    {
        bool Create(Withdraw withdraw, out string error);

        bool Delete(int id, out string error);

        ResultPage<Withdraw> ListPage(int? idCustomer, int page, int pageItems, string sort, out string error);

        Withdraw Read(int id, out string error);

        bool Update(Withdraw withdraw, out string error);
    }
}
