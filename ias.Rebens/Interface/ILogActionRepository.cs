using ias.Rebens.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ILogActionRepository
    {
        List<LogAction> ListByItem(Enums.LogItem item, int idItem, out string error);
        ResultPage<LogAction> ListByUser(int idAdminUser, int page, int pageItems, out string error, int? item);
        void Create(Enums.LogAction action, LogItem item, int idItem, int idAdminUser, out string error);
    }
}
