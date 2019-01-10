using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBenefitRepository
    {
        Benefit Read(int id, out string error);

        ResultPage<Benefit> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(Benefit benefit, out string error);

        bool Update(Benefit benefit, out string error);

        bool AddOperation(int idBenefit, int idOperation, out string error);

        bool AddAddress(int idBenefit, int idAddress, out string error);

        bool DeleteOperation(int idBenefit, int idOperation, out string error);

        bool DeleteAddress(int idBenefit, int idAddress, out string error);

        bool AddCategory(int idBenefit, int idCategory, out string error);

        bool DeleteCategory(int idBenefit, int idCategory, out string error);

        List<Benefit> ListByAddress(int idBenefit, out string error);

        List<Benefit> ListByCategory(int idBenefit, out string error);

        List<Benefit> ListByOperation(int idBenefit, out string error);

        List<Benefit> ListByType(int idType, out string error);

        List<Benefit> ListByPartner(int idPartner, out string error);

        List<Benefit> ListByIntegrationType(int idIntegrationType, out string error);

    }
}
