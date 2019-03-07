using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper
{
    public static class EmailHelper
    {
        public static bool SendPasswordRecovery(IStaticTextRepository staticTextRepo, Operation operation, Customer user, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.EmailPasswordRecovery, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                var link = operation.Domain + "#/?c=" + user.Code;
                var body = staticText.Html.Replace("##NAME##", user.Name).Replace("##LINK##", link);
                var result = sendingBlue.Send(user.Email, user.Name, "contato@rebens.com.br", operation.Title, "Recuperação de senha", body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendCustomerValidation(IStaticTextRepository staticTextRepo, Operation operation, Customer customer, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.EmailCustomerValidation, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string link = operation.Domain + "#/?c=" + customer.Code;
                var body = staticText.Html.Replace("##LINK##", link);
                var result = sendingBlue.Send(customer.Email, "", "contato@rebens.com.br", operation.Title, "Confirmação de e-mail", body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendCustomerReferal(IStaticTextRepository staticTextRepo, Operation operation, Customer customer, CustomerReferal referal, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.EmailDefault, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                var msg = $"<p>Olá {referal.Name}<br /><br />Você foi convidado para participar do clube: {operation.Title}</p>";
                msg += $"<p>Clique no link, para se cadastrar: <a href='{operation.Domain}'>{operation.Domain}</a></p>";
                string body = staticText.Html.Replace("###BODY###", msg);
                var result = sendingBlue.Send(referal.Email, referal.Name, "contato@rebens.com.br", "Contato", "Indicação - " + operation.Title, body);
                if (result.Status)
                    return true;
                error = result.Message;
            }

            return false;
        }
    }
}
