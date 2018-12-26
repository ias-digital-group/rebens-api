using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IPermissionRepository
    {
        List<Permission> ListTree(out string error);

        ResultPage<Permission> ListPage(int page, int pageItems, out string error);

        ResultPage<Permission> SearchPage(string word, int page, int pageItems, out string error);

        bool Create(Permission permission, out string error);

        bool Update(Permission permission, out string error);

        bool Delete(int id, out string error);

        Permission Read(int id, out string error);
    }
}
