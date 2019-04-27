using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationCustomerRepository
    {
        OperationCustomer ReadByCpf(string cpf, int idOperation, out string error);
        bool SetSigned(int id, out string error);
        OperationCustomer ReadByEmail(string email, int idOperation, out string error);
        ResultPage<OperationCustomer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null);
        bool Create(OperationCustomer operationCustomer, out string error);
        bool Update(OperationCustomer operationCustomer, out string error);
        bool Delete(int id, out string error);
    }
}
