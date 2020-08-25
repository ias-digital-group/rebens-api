using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IZanoxIncentiveRepository
    {
        bool Save(ZanoxIncentive incentive, out string error);
        ResultPage<ZanoxIncentive> ListPage(int page, int pageItems, string word, string sort, out string error);
        bool DisableIncentives(List<int> incentiveIds);
    } 
}
