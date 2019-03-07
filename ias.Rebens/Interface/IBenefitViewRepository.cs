using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IBenefitViewRepository
    {
        bool SaveView(int idBenefit, int idCustomer, out string error);
    }
}
