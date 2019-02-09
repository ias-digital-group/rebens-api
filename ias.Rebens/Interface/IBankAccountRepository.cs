using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBankAccountRepository
    {
        List<Bank> ListBanks(out string error);

        BankAccount Read(int id, out string error);

        ResultPage<BankAccount> ListPage(int idCustomer, int page, int pageItems, string word, string sort, out string error);

        bool Create(BankAccount account, out string error);

        bool Update(BankAccount account, out string error);

        bool Delete(int id, out string error);
    }
}
