using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IProfileRepository
    {
        ResultPage<Profile> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Create(Profile profile, out string error);

        bool Update(Profile profile, out string error);

        bool Delete(int id, out string error);

        Profile Read(int id, out string error);
    }
}
