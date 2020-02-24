using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ias.Rebens
{
    public interface IScratchcardRepository
    {
        bool Create(Scratchcard scratchcard, int idAdminUser, out string error);
        bool Update(Scratchcard scratchcard, int idAdminUser, out string error);
        bool Delete(int id, int idAdminUser, out string error);
        ResultPage<Scratchcard> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation);
        Task<bool> GenerateScratchcards(int id, int idAdminUser, string path);
    }
}
