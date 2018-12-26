using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ILogErrorRepository
    {
        int Create(LogError log);

        bool Clear();

        bool DeleteOlderThan(DateTime date);

        ResultPage<LogError> ListPage(int page, int pageItems);
    }
}
