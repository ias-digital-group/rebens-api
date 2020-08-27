using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IZanoxProgramRepository
    {
        bool Save(ZanoxProgram program, out string error, int? idAdminUser = null);
        ResultPage<ZanoxProgram> ListPage(int page, int pageItems, string word, string sort, out string error);
        ResultPage<ZanoxProgram> ListPageForPortal(int page, int pageItems, string word, out string error);
        ZanoxProgram Read(int id, out string error);
        void SaveView(int id, int idCustomer, out string error);
        bool TogglePublish(int id, int idAdminUser, out string error);
    }
}
