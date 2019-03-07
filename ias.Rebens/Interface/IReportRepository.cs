using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IReportRepository
    {
        Dashboard LoadDashboard(out string error, DateTime? begin = null, DateTime? end = null, int? idOperation = null);
    }
}
