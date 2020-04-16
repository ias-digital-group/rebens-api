using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ias.Rebens.Helper
{
    public static class EmailHelper
    {
        public static bool SendPasswordRecovery(IStaticTextRepository staticTextRepo, Operation operation, string emailFrom, Customer user, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.EmailPasswordRecovery, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                var link = (string.IsNullOrEmpty(operation.Domain) ? (operation.TemporarySubdomain + ".sistemarebens.com.br") : operation.Domain) + "#/?c=" + user.Code;
                var body = staticText.Html.Replace("##NAME##", user.Name).Replace("##LINK##", link);
                var listDestinataries = new Dictionary<string, string> { { user.Email, user.Name } };
                var result = sendingBlue.Send(listDestinataries, emailFrom, operation.Title, "Recuperação de senha", body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendCustomerValidation(IStaticTextRepository staticTextRepo, Operation operation, Customer customer, string emailFrom, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.EmailCustomerValidation, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string link = (string.IsNullOrEmpty(operation.Domain) ? (operation.TemporarySubdomain + ".sistemarebens.com.br") : operation.Domain) + "/#/?c=" + customer.Code;
                var body = staticText.Html.Replace("##LINK##", link);
                var listDestinataries = new Dictionary<string, string> { { customer.Email, "" } };
                var result = sendingBlue.Send(listDestinataries, emailFrom, operation.Title, "Confirmação de e-mail", body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendCustomerReferal(IStaticTextRepository staticTextRepo, Operation operation, Customer customer, CustomerReferal referal, string emailFrom, string color, out string error)
        {
            var staticText = staticTextRepo.ReadByType(operation.Id, (int)Enums.StaticTextType.Email, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string msg = "";
                string domain = (string.IsNullOrEmpty(operation.Domain) ? operation.TemporarySubdomain + ".sistemarebens.com.br" : operation.Domain);
                if (operation.Id == 1)
                {
                    msg = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {referal.Name}<br /><br />Você foi convidado para participar do {operation.Title}</p>";
                    msg += $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Clique no botão abaixo para se cadastrar</p><br /><br />";
                    msg += $"<p style='text-align:center;'><a href='{domain}' target='_blank' style='display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;font-size: 14px;padding: 15px 50px;font-family:verdana, arial, Helvetica; color: #ffffff;background-color:#427147;border-radius:50px;'>CADASTRAR</a></p>";
                }
                else
                {
                    msg = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá {referal.Name}</p><br /><br />";
                    msg += "<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Você foi convidado por um dos nossos participantes para ingressar em um Clube de Vantagens Exclusivo.</p><br />";
                    msg += "<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Clique no botão <b>“Quero fazer parte”</b>.</p>";
                    msg += $"<p style='text-align:center'><a href='{domain}' style='display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:{color};color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;'>QUERO ME CADASTRAR</a></p>";
                }
                //else
                //{
                //    msg = $"<p>Olá {referal.Name}</p><br /><br />";
                //    msg += "<p><b>Você foi convidado</b> por um dos nossos participantes <b>para ingressar</b> em um <b>Clube de Vantagens Exclusivo</b>.</p><br />";
                //    msg += $"<p>Clique no link, para se cadastrar: <a href='{domain}'>{domain}</a></p>";
                //}
                string body = staticText.Html.Replace("###BODY###", msg);
                var listDestinataries = new Dictionary<string, string> { { referal.Email, referal.Name } };
                var result = sendingBlue.Send(listDestinataries, emailFrom, "Contato", "Indicação - " + operation.Title, body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendCourseVoucher(StaticText staticText, Customer customer, Order order, Operation operation, string emailFrom, out string error)
        {
            error = null;
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string items = "";
                foreach (var item in order.OrderItems)
                    items += item.Name;
                var constant = new Constant();
                string msg = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {customer.Name}<br /><br />O pagamento do seu curso {items}</p>";
                msg += $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Clique no botão abaixo para baixar a sua carta de aprovação de bolsa de estudos</p><br /><br />";
                msg += $"<p style='text-align:center;'><a href='{constant.URL}Voucher/Course/{order.WirecardId}' target='_blank' style='display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;font-size: 14px;padding: 15px 50px;font-family:verdana, arial, Helvetica; color: #ffffff;background-color:#427147;border-radius:50px;'>BAIXAR</a></p>";
                
                string body = staticText.Html.Replace("###BODY###", msg);
                var listDestinataries = new Dictionary<string, string> { { customer.Email, customer.Name } };
                var result = sendingBlue.Send(listDestinataries, emailFrom, operation.Title, $"Pagamento Confirmado - PEDIDO #{order.DispId}", body);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendDefaultEmail(IStaticTextRepository staticTextRepo, string toEmail, string toName, int idOperation, string subject, string body, string emailFrom, string nameFrom, out string error)
        {
            var staticText = staticTextRepo.ReadByType(idOperation, (int)Enums.StaticTextType.Email, out error);
            if (staticText != null)
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                string message = staticText.Html.Replace("###BODY###", body);
                var listDestinataries = new Dictionary<string, string> { { toEmail, toName } };
                var result = sendingBlue.Send(listDestinataries, emailFrom, nameFrom, subject, message);
                if (result.Status)
                    return true;
                error = result.Message;
            }
            return false;
        }

        public static bool SendDefaultEmail(string toEmail, string toName, int idOperation, string subject, string body, string emailFrom, string nameFrom, out string error)
        {
            error = "";
            var sendingBlue = new Integration.SendinBlueHelper();
            var listDestinataries = new Dictionary<string, string> { { toEmail, toName } };
            var result = sendingBlue.Send(listDestinataries, emailFrom, nameFrom, subject, body);
            if (result.Status)
                return true;
            error = result.Message;
            return false;
        }

        public static bool SendDefaultEmail(string toEmail, string toName, string fromEmail, string fromName, string subject, string body, out string error)
        {
            error = "";
            var sendingBlue = new Integration.SendinBlueHelper();
            var listDestinataries = new Dictionary<string, string> { { toEmail, toName } };
            var result = sendingBlue.Send(listDestinataries, fromEmail, fromName, subject, body);
            if (result.Status)
                return true;
            error = result.Message;
            return false;
        }

        public static bool SendAdminEmail(Dictionary<string, string> listDestinataries, string subject, string body, out string error)
        {
            var sendingBlue = new Integration.SendinBlueHelper();

            string html = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"/><title>Rebens</title><style type=\"text/css\"> html,body{Margin:0 !important;padding:0 !important;height:100% !important;width:100% !important}*{-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%}div[style*=\"margin: 16px 0\"]{margin:0 !important}table,td{mso-table-lspace:0pt !important;mso-table-rspace:0pt !important}table{border-spacing:0 !important;border-collapse:collapse !important;table-layout:fixed !important;Margin:0 auto !important}table table table{table-layout:auto}img{-ms-interpolation-mode:bicubic}.yshortcuts a{border-bottom:none !important}.mobile-link--footer a,a[x-apple-data-detectors]{color:inherit !important;text-decoration:underline !important}@media screen and (max-width:600px){.fluid,.fluid-centered{width:100% !important;max-width:100% !important;height:auto !important;Margin-left:auto !important;Margin-right:auto !important}.fluid-centered{Margin-left:auto !important;Margin-right:auto !important}.stack-column,.stack-column-center{display:block !important;width:100% !important;max-width:100% !important;direction:ltr !important}.stack-column-center{text-align:center !important}.center-on-narrow{text-align:center !important;display:block !important;Margin-left:auto !important;Margin-right:auto !important;float:none !important}table.center-on-narrow{display:inline-block !important}*[class=\"mobileOff\"]{display:none !important;width:0px !important}}</style></head><body width=\"100%\" bgcolor=\"#eeeeee\" style=\"background-color:#eeeeee; Margin: 0\"><table id=\"maintable\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" height=\"100%\" width=\"100%\" bgcolor=\"#eeeeee\" style=\"border-collapse:collapse;max-width: 600px;\"><tr><td valign=\"top\"><center style=\"width:100%\"><div style=\"max-width: 600px;\"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"max-width: 600px\"><tr><td style=\"background-color:#ffffff; border-top: solid 4px #08061e; padding: 35px 0; border-bottom: solid 1px #f1f1f1;\"><img src=\"https://res.cloudinary.com/rebens/image/upload/v1556206797/rebens/udzqwqcqgovqppvulxyt.jpg\" alt=\"Rebens\" border=\"0\" style=\"border:0;display:block;margin:0 auto;padding:0;outline:none;text-decoration:none\"></td></tr></table><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" style=\"max-width: 600px\" bgcolor=\"#ffffff\"><tr><td style=\"padding: 45px 0\" class=\"stack-column\"><div style=\"display:block;padding-bottom:0;text-align:left; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; padding: 0 20px\"> ###BODY### </div></td></tr></table><table bgcolor=\"\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"max-width:600px;border-top: solid 1px #f1f1f1;\"><tr><td style=\"border-bottom: solid 4px #08061e\" bgcolor=\"#ffffff\" align=\"center\" height=\"100%\" valign=\"top\" width=\"100%\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\" width=\"100%\" style=\"max-width:600px;\"><tr><td align=\"center\" valign=\"top\" style=\"font-size:0;padding:20px; \"><div style=\"display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;\" class=\"stack-column\"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\"><tr><td><table class=\"stack-column\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" style=\"font-size:14px;text-align: left;\"><tr><td class=\"stack-column\" bgcolor=\"#ffffff\" style=\"text-align:left;\"><p style=\"display:block;padding-bottom:0;text-align:left; font-size: 10px; font-family: arial, Helvetica; color: #666666;\"><strong>Rebens - CNPJ: 21.296.610/0001-54</strong><br>Rua Haddock Lobo, 1307 - Cj 82 <br>Cerqueira César - São Paulo/SP - 04551-000</p></td></tr></table></td></tr></table></div><div style=\"display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;\" class=\"stack-column\"><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\"><tr><td><table class=\"stack-column\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" style=\"font-size:14px;text-align: right;\"><tr><td class=\"stack-column\" bgcolor=\"#ffffff\" style=\"text-align:right;\"><a href=\"http://rebens.com.br\" target=\"_blank\" style=\"display:block;margin:0;outline:none;padding-bottom:0;text-align:right;text-decoration:none\"><img src=\"https://res.cloudinary.com/rebens/image/upload/v1557179427/Portal/logo-footer.jpg\" width=\"129\" alt=\"alt_text\" border=\"0\" style=\"border:0;display:inline-block;padding:0;max-width:129px;\"></a></td></tr></table></td></tr></table></div></td></tr></table></td></tr></table><br/></div></center></td></tr></table></body></html>";
            html = html.Replace("###BODY###", body);

            var result = sendingBlue.Send(listDestinataries, "contato@rebens.com.br", "Contato", subject, html);
            if (result.Status)
            {
                error = null;
                return true;
            }
            error = result.Message;
            return false;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
