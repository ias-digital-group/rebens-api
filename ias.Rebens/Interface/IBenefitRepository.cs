using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBenefitRepository
    {
        Benefit Read(int id, out string error);

        ResultPage<Benefit> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null, int? type = null, bool exclusive = false);

        bool Delete(int id, int idAdminUser, out string error);

        bool Create(Benefit benefit, int idAdminUser, out string error);

        bool Update(Benefit benefit, int idAdminUser, out string error);

        bool AddOperation(int idBenefit, int idOperation, int idPostion, int idAdminUser, out string error);

        bool AddAddress(int idBenefit, int idAddress, int idAdminUser, out string error);

        bool DeleteOperation(int idBenefit, int idOperation, int idAdminUser, out string error);

        bool DeleteAddress(int idBenefit, int idAddress, int idAdminUser, out string error);

        bool AddCategory(int idBenefit, int idCategory, int idAdminUser, out string error);

        bool DeleteCategory(int idBenefit, int idCategory, int idAdminUser, out string error);

        ResultPage<Benefit> ListByAddress(int idAddress, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByCategory(int idCategory, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByType(int idType, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByIntegrationType(int idIntegrationType, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByOperation(int idOperation, int? idCategory, string benefitTypes, decimal? latitude, decimal? longitude, int page, int pageItems, string word, string sort, string idBenefits, string state, string city, out string error);

        List<BenefitOperationPosition> ListPositions(out string error);

        void ReadCallAndPartnerLogo(int idBenefit, out string title, out string call, out string logo, out string error);

        List<Benefit> ListActive(out string error);

        bool SaveCategories(int idBenefit, string categoryIds, int idAdminUser, out string error);

        ResultPage<Benefit> ListForHomePortal(int idOperation, out string error);

        ResultPage<Benefit> ListForHomeBenefitPortal(int idOperation, out string error);

        List<Tuple<string, string>> ListStates(int idOperation, out string error);
        
        List<Tuple<string, string>> ListCities(int idOperation, out string error, string state = null);

        bool ToggleActive(int id, int idAdminUser, out string error);

        bool Duplicate(int id, out int newId, int idAdminUser, out string error);

        List<Benefit> ListToCheckLinks();

        bool ConnectOperations(int id, int[] operations, out string error);
    }
}
