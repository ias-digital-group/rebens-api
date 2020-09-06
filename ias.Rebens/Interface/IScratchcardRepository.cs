using ias.Rebens.Entity;
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
        ResultPage<ScratchcardListItem> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation, int? status);
        Task<bool> GenerateScratchcards(int id, int idAdminUser, string path);
        Scratchcard Read(int id, out string error);
        Scratchcard Read(int id, out bool canPublish, out string regulation, out string error);
        List<Scratchcard> ListByDistributionType(Enums.ScratchcardDistribution type);
        List<int> ListCustomers(int idOperation, int type);
    }
}
