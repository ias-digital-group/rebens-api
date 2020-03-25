using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationRepository
    {
        string GetName(int id, out string error);

        Operation Read(int id, out string error);
        
        Operation Read(Guid code, out string error);

        Operation ReadForSignUp(Guid code, out bool openSignUp, out string error);

        ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null);

        bool Create(Operation operation, int idAdminUser, out string error);

        bool Update(Operation operation, int idAdminUser, out string error);

        bool AddContact(int idOperation, int idContact, int idAdminUser, out string error);

        bool AddAddress(int idOperation, int idAddress, int idAdminUser, out string error);

        bool DeleteContact(int idOperation, int idContact, int idAdminUser, out string error);

        bool DeleteAddress(int idOperation, int idAddress, int idAdminUser, out string error);

        List<BenefitOperationItem> ListByBenefit(int idBenefit, out string error);

        ResultPage<Operation> ListByBanner(int idBanner, int page, int pageItems, string word, string sort, out string error);

        List<BannerOperationItem> ListByBanner(int idBanner, out string error);

        bool SavePublishStatus(int id, int idStatus, int idAdminUser, int? idError, out string error);

        bool ValidateOperation(int id, out string error);

        object GetPublishData(int id, out string domain, out string error);

        bool SavePublishDone(Guid code, out string error);

        bool SetSubdomainCreated(int id, out string error);

        string GetConfigurationOption(int id, string field, out string error);
        
        int GetId(Guid operationGuid, out string error);

        bool SaveSendingblueListId(int id, int listId, out string error);

        string LoadModulesNames(int id, out string error);
        List<Operation> ListWithModule(string module, out string error);
    }
}
