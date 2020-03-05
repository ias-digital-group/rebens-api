using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IScratchcardDrawRepository
    {
        ResultPage<ScratchcardDraw> ListByCustomer(int idCustomer, int page, int pageItems, out string error);
        ResultPage<ScratchcardDraw> ListByScratchcard(int idScratchcard, int page, int pageItems, out string error);
        ScratchcardDraw Read(int id, out string error);
        bool SaveRandom(int idScratchcard, string path, int idCustomer, DateTime date, DateTime? expireDate, out string error);
    }
}
