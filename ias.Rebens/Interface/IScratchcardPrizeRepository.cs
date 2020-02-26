using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IScratchcardPrizeRepository
    {
        bool Create(ScratchcardPrize prize, int idAdminUser, out string error);
        bool Update(ScratchcardPrize prize, int idAdminUser, out string error);
        bool Delete(int id, int idAdminUser, out string error);
        List<ScratchcardPrize> List(int idScratchcard, out string error);
        ScratchcardPrize Read(int id, out string error);
    }
}
