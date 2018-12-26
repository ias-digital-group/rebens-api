using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationTypeRepository
    {
        bool Create(OperationType operationType, out string error);

        bool Update(OperationType operationType, out string error);

        bool Delete(int id, out string error);

        OperationType Read(int id, out string error);

        List<OperationType> ListActive(out string error);

        List<OperationType> List(out string error);
    }
}
