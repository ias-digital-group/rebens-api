using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOrderRepository
    {
        bool Create(Order order, out string error);
        bool SaveWirecardInfo(int id, string wirecardId, string status, out string error);
        Order Read(int id, out string error);
        ResultPage<Order> ListByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);
        List<Order> ListToUpdate(int count, out string error);
        Order ReadByWirecardId(string id, out string error);

        bool SendOrderConfirmationEmail(int idOrder, out string error);
    }
}
