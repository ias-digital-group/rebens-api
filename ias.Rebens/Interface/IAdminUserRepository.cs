using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IAdminUserRepository
    {
        ResultPage<AdminUser> ListPage(string userRole, int page, int pageItems, string word, string sort, out string error, int? idOperation = null, bool? status = null, string role = null, int? idOperationPartner = null);

        bool Create(AdminUser adminUser, int idAdminUser, out string error);

        bool Update(AdminUser adminUser, int idAdminUser, out string error);

        bool Delete(int id, int idAdminUser, out string error);

        AdminUser Read(int id, out string error);
        AdminUser ReadFull(int id, out string error);

        AdminUser ReadByEmail(string email, out string error);

        bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, out string error);

        bool SetLastLogin(int id, out string error);
        
        bool ToggleActive(int id, int idAdminUser, out string error);

        bool CheckCPF(int id, string cpf, out string error);
        bool CheckEmail(int id, string email, out string error);
    }
}
