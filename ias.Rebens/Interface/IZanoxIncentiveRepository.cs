using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IZanoxIncentiveRepository
    {
        bool Save(ZanoxIncentive incentive, out string error);
        ResultPage<ZanoxIncentive> ListPage(int page, int pageItems, string word, out string error, int? idZanoxProgram = null);
        bool DisableIncentives(List<int> incentiveIds);
        void SaveClick(int id, int idCustomer, out string error);
    } 
}
