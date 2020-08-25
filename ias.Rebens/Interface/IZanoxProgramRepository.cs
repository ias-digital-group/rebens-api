using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IZanoxProgramRepository
    {
        bool Save(ZanoxProgram program, out string error);
        ResultPage<ZanoxProgram> ListPage(int page, int pageItems, string word, string sort, out string error);
    }
}
