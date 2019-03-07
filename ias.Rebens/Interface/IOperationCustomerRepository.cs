using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationCustomerRepository
    {
        OperationCustomer ReadByCpf(string cpf, out string error);
        bool SetSigned(int id, out string error);
    }
}
