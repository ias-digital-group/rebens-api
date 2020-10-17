using ias.Rebens.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IReportRepository
    {
        Dashboard LoadDashboard(out string error, DateTime? begin = null, DateTime? end = null, int? idOperation = null);
        ResultPage<CustomerReportItem> ListCustomerPage(int page, int pageItems, string searchWord, string sort, out string error, int? idOperation, int? idPartner, int? status);
        ResultPage<BenefitReportItem> ListBenefitUsePage(int page, int pageItems, string word, string sort, out string error, int? idOperation, DateTime? start, DateTime? end);
        UnicsulReport UnicsulWeekly();
    }
}
