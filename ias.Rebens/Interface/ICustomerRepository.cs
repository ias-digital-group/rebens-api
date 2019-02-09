using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface ICustomerRepository
    {
        string ReadConfigurationValuesString(int idCustomer, out string error);

        List<Helper.Config.ConfigurationValue> ListConfigurationValues(int idCustomer, out string error);

        Customer Read(int id, out string error);

        ResultPage<Customer> ListPage(int? idOperation, int page, int pageItems, string word, string sort, out string error);

        bool Create(Customer customer, out string error);

        bool Update(Customer customer, out string error);

        bool Delete(int id, out string error);

        Customer ReadByEmail(string email, int idOperation, out string error);

        bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, out string error);
    }
}
