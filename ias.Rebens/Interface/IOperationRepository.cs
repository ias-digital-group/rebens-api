using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationRepository
    {
        Operation Read(int id, out string error);

        Operation Read(Guid code, out string error);

        Operation ReadForSignUp(Guid code, out bool openSignUp, out string error);

        ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error, bool? status = null);

        bool Create(Operation operation, out string error);

        bool Update(Operation operation, out string error);

        bool AddContact(int idOperation, int idContact, out string error);

        bool AddAddress(int idOperation, int idAddress, out string error);

        bool DeleteContact(int idOperation, int idContact, out string error);

        bool DeleteAddress(int idOperation, int idAddress, out string error);

        List<BenefitOperationItem> ListByBenefit(int idBenefit, out string error);

        ResultPage<Operation> ListByBanner(int idBanner, int page, int pageItems, string word, string sort, out string error);

        List<BannerOperationItem> ListByBanner(int idBanner, out string error);

        bool SavePublishStatus(int id, int idStatus, int? idError, out string error, bool isTemporary = false);

        bool ValidateOperation(int id, out string error, bool isTemporary = false);

        object GetPublishData(int id, bool isTemporary, out string error);

        bool SavePublishDone(Guid code, out string error);

        bool SetSubdomainCreated(int id, out string error);

        string GetConfigurationOption(int id, string field, out string error);
    }
}
