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
        Order ReadByItem(string code, string voucher, out string error);
        bool SendOrderConfirmationEmail(int idOrder, out string error);
        bool HasOrderToProcess();
        void ProcessOrder();
        Order ReadByDispId(string id, out string error);
        Order ReadByWirecardId(string id, out string error);
        ResultPage<Entity.ProductValidateItem> ListItemsByOperation(int page, int pageItems, string word, out string error, int? idOperation = null);
        bool SetItemUsed(int id, int idAdminUser, out string error);
    }
}
