using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICustomerPromoterRepository
    {
        ResultPage<Customer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idStatus = null, int? idOperation = null, int? idPromoter = null);
        Customer Read(int id, out string error);
        bool Create(Customer customer, int idPromoter, out string error);
        ResultPage<PromoterReportModel> Report(int page, int pageItems, string word, out string error, int? idOperation = null);
    }
}
