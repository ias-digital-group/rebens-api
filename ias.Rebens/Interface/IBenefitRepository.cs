﻿using System;
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

        bool AddOperation(int idBenefit, int idOperation, int idPostion, out string error);

        bool AddAddress(int idBenefit, int idAddress, out string error);

        bool DeleteOperation(int idBenefit, int idOperation, out string error);

        bool DeleteAddress(int idBenefit, int idAddress, out string error);

        bool AddCategory(int idBenefit, int idCategory, out string error);

        bool DeleteCategory(int idBenefit, int idCategory, out string error);

        ResultPage<Benefit> ListByAddress(int idAddress, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByCategory(int idCategory, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByType(int idType, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByIntegrationType(int idIntegrationType, int page, int pageItems, string word, string sort, out string error);

        ResultPage<Benefit> ListByOperation(int idOperation, int? idCategory, string benefitTypes, int page, int pageItems, string word, string sort, out string error);

        List<BenefitOperationPosition> ListPositions(out string error);

        void ReadCallAndPartnerLogo(int idBenefit, out string title, out string call, out string logo, out string error);

        List<Benefit> ListActive(out string error);

        bool SaveCategories(int idBenefit, string categoryIds, out string error);

        ResultPage<Benefit> ListForHomePortal(int idOperation, out string error);

        ResultPage<Benefit> ListForHomeBenefitPortal(int idOperation, out string error);
    }
}
