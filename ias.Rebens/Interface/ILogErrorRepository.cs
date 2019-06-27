using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ILogErrorRepository
    {
        int Create(LogError log);
        int Create(string reference, string message, string complement, string stackTrace);

        bool Clear();

        bool DeleteOlderThan(DateTime date);

        ResultPage<LogError> ListPage(int page, int pageItems);
    }
}
