using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IAdminUserRepository
    {
        ResultPage<AdminUser> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Create(AdminUser adminUser, out string error);

        bool Update(AdminUser adminUser, out string error);

        bool Delete(int id, out string error);

        AdminUser Read(int id, out string error);

        AdminUser ReadByEmail(string email, out string error);

        bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, out string error);

        bool SetLastLogin(int id, out string error);
    }
}
