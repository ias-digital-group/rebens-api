using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBenefitTypeRepository
    {
        bool Create(BenefitType benefitType, out string error);

        bool Update(BenefitType benefitType, out string error);

        bool Delete(int id, out string error);

        BenefitType Read(int id, out string error);

        List<BenefitType> ListActive(out string error);

        List<BenefitType> List(out string error);
    }
}
