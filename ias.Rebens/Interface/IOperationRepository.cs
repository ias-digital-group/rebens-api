using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationRepository
    {
        Operation Read(int id, out string error);

        ResultPage<Operation> ListPage(int page, int pageItems, out string error);

        ResultPage<Operation> SearchPage(string word, int page, int pageItems, out string error);

        bool Create(Operation operation, out string error);

        bool Update(Operation operation, out string error);
    }
}
