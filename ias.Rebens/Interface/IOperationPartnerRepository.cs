using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationPartnerRepository
    {
        OperationPartner Read(int id, out string error);

        ResultPage<OperationPartner> ListPage(int page, int pageItems, string word, string sort, int idOperation, out string error, bool? status = null);

        bool Delete(int id, out string error);

        bool Create(OperationPartner partner, out string error);

        bool Update(OperationPartner partner, out string error);

        bool CreateCustomer(OperationPartnerCustomer customer, out string error);

        bool DeleteCustomer(int idCustomer, out string error);

        bool UpdateCustomerStatus(int idCustomer, int status, out string error, out Operation operation, out Customer customer);

        OperationPartnerCustomer ReadCustomer(int idCustomer, out string error);

        ResultPage<OperationPartnerCustomer> ListCustomers(int page, int pageItems, string word, string sort, out string error, int? status = null, int? idOperationPartner = null, int? idOperation = null);

        List<OperationPartner> ListActiveByOperation(Guid operationCode, out string error);

        Dictionary<string, string> ListDestinataries(int idOperationPartner, out string error);
    }
}
