using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICustomerReferalRepository
    {
        CustomerReferal Read(int id, out string error);

        ResultPage<CustomerReferal> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(CustomerReferal customerReferal, out string error);

        bool Update(CustomerReferal customerReferal, out string error);

        ResultPage<CustomerReferal> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);

        bool ChangeStatus(int id, Enums.CustomerReferalStatus status, out string error);
    }
}
