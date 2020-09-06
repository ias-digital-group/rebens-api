using ias.Rebens.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IScratchcardDrawRepository
    {
        ResultPage<ScratchcardDraw> ListByCustomer(int idCustomer, int page, int pageItems, out string error);
        ResultPage<ScratchcardDraw> ListByScratchcard(int idScratchcard, int page, int pageItems, out string error);
        ResultPage<ScratchcardDrawListItem> ScratchedWithPrizeListPage(int page, int pageItems, string searchWord, int? idOperation, int? idScratchcard, out string error);
        ScratchcardDraw Read(int id, out string error);
        bool SaveRandom(int idScratchcard, string path, int idCustomer, DateTime date, DateTime? expireDate, out string error);
        bool SetOpened(int id, int idCustomer);
        bool SetPlayed(int id, int idCustomer, int idOperation);
        bool Validate(int id, int idCustomer);
        ResultPage<ScratchcardDrawListItem> ListPage(int page, int pageItems, string searchWord, out string error, int? idOperation, int? idScratchcard);
    }
}
