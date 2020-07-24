using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationPartnerRepository
    {
        OperationPartner Read(int id, out string error);

        ResultPage<Entity.OperationPartnerListItem> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null, int? idOperation = null);

        bool Delete(int id, out string error);

        bool Create(OperationPartner partner, out string error);

        bool Update(OperationPartner partner, out string error);

        List<OperationPartner> ListActiveByOperation(Guid operationCode, out string error);

        Dictionary<string, string> ListDestinataries(int idOperationPartner, out string error);

        bool ToggleActive(int id, int idAdminUser, out string error);
    }
}
