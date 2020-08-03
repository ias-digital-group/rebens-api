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
        ResultPage<Customer> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null, 
                                        int? idOperationPartner = null, int? status = null, bool? active = null, int? idPromoter = null);
        bool Create(Customer customer, out string error);
        bool Create(Customer customer, int idAdminUser, out string error);
        bool Update(Customer customer, int idAdminUser, out string error);
        bool Update(Customer customer, out string error);
        bool Delete(int id, int idAdminUser, out string error);
        Customer ReadByEmail(string email, int idOperation, out string error);
        bool ChangePassword(int id, string passwordEncrypted, string passwordSalt, int? status, out string error);
        Customer ReadByCode(string code, int idOperation, out string error);
        bool ChangeStatus(int id, Enums.CustomerStatus status, out string error);
        bool CheckEmailAndCpf(string email, string cpf, int idOperation, out string error);
        List<Customer> ListToGenerateCoupon(int idOperation, int totalItems);
        bool HasToGenerateCoupon(int idOperation);
        MoipSignature CheckPlanStatus(int id);
        bool SaveSendingblueId(int id, int blueId, out string error);
        bool ToggleActive(int id, int idAdminUser, out string error);
        Customer ReadPreSign(string cpf, int idOperation, out string error);
        bool ChangeComplementaryStatus(int id, Enums.CustomerComplementaryStatus status, out string error);
        void SaveLog(int id, Enums.CustomerLogAction action, string extra);

        #region Referal
        bool CheckReferalLimit(int idOperation, int idCustomer, out int limit, out string error);
        ResultPage<Customer> ListReferalByCustomer(int idCustomer, int page, int pageItems, string word, string sort, out string error);
        ResultPage<Customer> ListReferalPage(int page, int pageItems, string searchWord, string sort, int? idOperation, out string error);

        bool DeleteReferal(int id, out string error);
        #endregion Referal

        #region Promoter
        ResultPage<PromoterReportModel> Report(int page, int pageItems, string word, out string error, int? idOperation = null);
        #endregion Promoter
    }
}
