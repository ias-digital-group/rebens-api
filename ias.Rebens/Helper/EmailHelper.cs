﻿using System;
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
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.Email, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string msg = "";
                if (operation.Id == 1)
                {
                    msg = $"<p>Olá {referal.Name}<br /><br />Você foi convidado para participar do clube: {operation.Title}</p>";
                    msg += $"<p>Clique no link, para se cadastrar: <a href='{operation.Domain}'>{operation.Domain}</a></p>";
                }
                else if(operation.Id == 2)
                {
                    msg = $"<p>Olá {referal.Name}</p><br /><br />";
                    msg += "<p><b>Você foi convidado</b> por um dos nossos participantes <b>para ingressar</b> em um <b>Clube de Vantagens Exclusivo</b>.</p><br />";
                    msg += $"<p><a href='{operation.Domain}' style='display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#00b0d3;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;'>QUERO ME CADASTRAR</a></p>";
                }
                else
                {
                    msg = $"<p>Olá {referal.Name}</p><br /><br />";
                    msg += "<p><b>Você foi convidado</b> por um dos nossos participantes <b>para ingressar</b> em um <b>Clube de Vantagens Exclusivo</b>.</p><br />";
                    msg += $"<p>Clique no link, para se cadastrar: <a href='{operation.Domain}'>{operation.Domain}</a></p>";
                }
                string body = staticText.Html.Replace("###BODY###", msg);
                var result = sendingBlue.Send(referal.Email, referal.Name, "contato@rebens.com.br", "Contato", "Indicação - " + operation.Title, body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendDefaultEmail(IStaticTextRepository staticTextRepo, string toEmail, string toName, int idOperation, string subject, string body, out string error)
        {
            var staticText = staticTextRepo.ReadByType(idOperation, (int)Enums.StaticTextType.Email, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string message = staticText.Html.Replace("###BODY###", body);
                var result = sendingBlue.Send(toEmail, toName, "contato@rebens.com.br", "Contato", subject, message);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendAdminEmail(string toEmail, string toName, string subject, string body, out string error)
        {
            var sendingBlue = new Integration.SendinBlueHelper();
            var result = sendingBlue.Send(toEmail, toName, "contato@rebens.com.br", "Contato", subject, body);
            if (result.Status)
            {
                error = null;
                return true;
            }
            error = result.Message;
            return false;
        }
    }
}
