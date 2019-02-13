using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ias.Rebens.Integration
{
    public class SendinBlueHelper
    {
        const string API_KEY = "dMt0h1FYL4yaOAIG";

        public Models.SendinBlueModel Send(string toEmail, string toName, string fromEmail, string fromName, string subject, string body)
        {
            var resultModel = new Models.SendinBlueModel();

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(toEmail, toName);
            List<string> from_name = new List<string>();
            from_name.Add(fromEmail);
            from_name.Add(fromName);
            //List<string> attachment = new List<string>();
            //attachment.Add("https://domain.com/path-to-file/filename1.pdf");
            //attachment.Add("https://domain.com/path-to-file/filename2.jpg");

            data.Add("to", to);
            data.Add("from", from_name);
            data.Add("subject", subject);
            data.Add("html", body);

            string content = JsonConvert.SerializeObject(data);
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create("https://api.sendinblue.com/v2.0/email") as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 30000;
            request.Headers.Add("api-key", API_KEY);

            using (Stream s = request.GetRequestStream())
            {
                using (StreamWriter sw = new StreamWriter(s))
                    sw.Write(content);
            }
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream() as Stream;
                    byte[] buffer = new byte[32 * 1024];
                    int nRead = 0;
                    MemoryStream successMs = new MemoryStream();
                    do
                    {
                        nRead = stream.Read(buffer, 0, buffer.Length);
                        successMs.Write(buffer, 0, nRead);
                    } while (nRead > 0);
                    // convert read bytes into string
                    var responseString = encoding.GetString(successMs.ToArray());
                    var jObj = JObject.Parse(responseString);

                    resultModel.Status = jObj["code"].ToString() == "success";
                    resultModel.Message = jObj["message"].ToString();
                }
                else
                {
                    resultModel.Status = false;
                    resultModel.Message = "Ocorreu um erro ao tentar enviar o e-mail";
                }
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response == null)
                {
                    resultModel.Status = false;
                    resultModel.Message = "Ocorreu um erro ao tentar enviar o e-mail";
                }
            }
            return resultModel;
        }

        public Models.SendinBlueModel SendCustomerValidate(string email, string link)
        {
            return Send(email, "", "contato@rebens.com.br", "Contato", "[Rebens] - Validação de cadastro", ValidationBody.Replace("##LINK##", link));
        }

        public Models.SendinBlueModel SendPasswordRecover(Customer customer, string link)
        {
              return Send(customer.Email, customer.Name, "contato@rebens.com.br", "Contato", "[Rebens] - Recuperação de Senha", PasswordRecoverBody.Replace("##LINK##", link));
        }

        #region  MailTemplate
        private readonly string PasswordRecoverBody = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
	<meta charset=""utf-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
	<title>Unicap</title>
    <!--[if mso]>
        <style>
            * {
                font-family: sans-serif !important;
            }
        </style>
    <![endif]-->
    <!--[if !mso]><!-->
        <!-- insert web font reference, eg: <link href='https://fonts.googleapis.com/css?family=Roboto:400,700' rel='stylesheet' type='text/css'> -->
    <!--<![endif]-->
    <style type=""text/css"">
        html,body{Margin:0 !important;padding:0 !important;height:100% !important;width:100% !important}
        *{-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%}
        div[style*=""margin: 16px 0""]{margin:0 !important}
        table,td{mso-table-lspace:0pt !important;mso-table-rspace:0pt !important}
        table{border-spacing:0 !important;border-collapse:collapse !important;table-layout:fixed !important;Margin:0 auto !important}
        table table table{table-layout:auto}
        img{-ms-interpolation-mode:bicubic}
        .yshortcuts a{border-bottom:none !important}
        .mobile-link--footer a,a[x-apple-data-detectors]{color:inherit !important;text-decoration:underline !important}
    </style>
    <style type=""text/css"">
        @media screen and (max-width:600px) {
            .fluid,.fluid-centered{width:100% !important;max-width:100% !important;height:auto !important;Margin-left:auto !important;Margin-right:auto !important}
            .fluid-centered{Margin-left:auto !important;Margin-right:auto !important}
            .stack-column,.stack-column-center{display:block !important;width:100% !important;max-width:100% !important;direction:ltr !important}
            .stack-column-center{text-align:center !important}
            .center-on-narrow{text-align:center !important;display:block !important;Margin-left:auto !important;Margin-right:auto !important;float:none !important}
            table.center-on-narrow{display:inline-block !important}
            *[class=""mobileOff""]{display:none !important;width:0px !important}
        }
    </style>
</head>
<body width=""100%"" bgcolor=""#eeeeee"" style=""background-color:#eeeeee; Margin: 0"">
<table id=""maintable"" cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#eeeeee"" style=""border-collapse:collapse;max-width: 600px;"">
    <tr>
        <td valign=""top"">
            <center style=""width:100%"">
                <div style=""max-width: 600px;"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" align=""center"">
                    <tr>
                    <td>
                    <![endif]-->    
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width: 600px"">
        	            <tr>
        					<td style=""background-color:#ffffff; border-top: solid 4px #427147; padding: 35px 0; border-bottom: solid 1px #f1f1f1;"">
                                <a href=""javascript:void"" target=""_blank"" style=""display:block;margin:0;outline:none;padding-bottom:0;text-align:center;text-decoration:none"">
                                    <img src=""http://hmlrebens.iasdigitalgroup.com/unicap/images/logo.jpg"" alt=""Clube Unicap"" border=""0"" style=""border:0;display:block;margin:0 auto;padding:0;"">
                                </a>
        					</td>
        	            </tr>
                    </table>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px"" bgcolor=""#ffffff"">
                        <tr>
                            <td style=""padding: 45px 0"" class=""stack-column"">
                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""Margin: auto"">
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 25px 0 0 0;"">
                                            <p style=""display:inline-block;text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0; padding: 0 20px"">Olá, João.</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 25px 0 0 0;"">
                                            <p style=""display:inline-block;text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px"">Seguem as informações solicitadas para recuperar sucesso ao Clube Cruzeiro do Sul.</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 25px 0 25px 0;"">
                                            <a href=""##LINK##"" target=""_blank"" style=""display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#427147;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;"">
                                                CRIAR NOVA SENHA
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px"" bgcolor=""#f1f1f1"">
                        <tr>
                            <td class=""stack-column"">
                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""Margin: auto"">
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center;padding: 35px 0"">
                                            <p style=""display:block;padding-bottom:0;text-align:center; font-size: 12px; font-family:verdana, arial, Helvetica; color: #427147; margin: 0; font-weight: bold;text-transform: uppercase; padding: 0 20px;"">Se você não deseja redefinir sua senha ou não solicitou estas<br> informações, pode ignorar este e-mail com segurança</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px"" bgcolor=""#ffffff"">
                        <tr>
                            <td style=""padding: 50px 0; border-bottom: solid 1px #f1f1f1"" class=""stack-column-center"">
                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""Margin: auto"">
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center;"">
                                            <p style=""display:block;padding-bottom:0;text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0; padding: 0 20px"">Este é um e-mail automático, por favor não responder.<br> Precisa falar com a gente?<a href=""javascript:void"" target=""_blank"" style=""margin:0;outline:none;padding-bottom:0;text-align:center;color: #427147;""> Clique aqui!</a></p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table bgcolor="""" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width:600px"">
                        <tr>
                            <td bgcolor=""#ffffff"" align=""center"" height=""100%"" valign=""top"" width=""100%"" style=""border-bottom: solid 4px #427147"">
                            <!--[if mso]>
                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" width=""600"">
                            <tr>
                            <td align=""center"" valign=""top"" width=""600"">
                            <![endif]-->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" style=""max-width:600px;"">
                                <tr>
                                    <td align=""center"" valign=""top"" style=""font-size:0;padding:20px;"">
                                    <!--[if mso]>
                                    <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" width=""560"">
                                    <tr>
                                    <td align=""left"" valign=""top"" width=""280"">
                                    <![endif]-->
                                    <div style=""display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;"" class=""stack-column"">
                                        
                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                            <tr>
                                                <td>
                                                    <table class=""stack-column"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""font-size:14px;text-align: left;"">
                                                        <tr>
                                                            <td class=""stack-column"" bgcolor=""#ffffff"" style=""text-align:left;"">
                                                                <p style=""display:block;padding-bottom:0;text-align:left; font-size: 10px; font-family: arial, Helvetica; color: #666666;""><strong>Rebens - CNPJ: 21.296.610/0001-54</strong><br>
                                                                    Rua Haddock Lobo, 1307 - Cj 82 <br>Cerqueira César - São Paulo/SP - 04551-000</p>
                                                            </td>
                                                        </tr> 
                                                    </table>
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <!--[if mso]>
                                    </td>
                                    <td align=""left"" valign=""top"" width=""280"">
                                    <![endif]-->
                                    <div style=""display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;"" class=""stack-column"">
                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                            <tr>
                                                <td>
                                                    <table class=""stack-column"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""font-size:14px;text-align: right;"">
                                                        <tr>
                                                            <td class=""stack-column"" bgcolor=""#ffffff"" style=""text-align:right;"">
                                                               <a href=""javascript:void"" target=""_blank"" style=""display:block;margin:0;outline:none;padding-bottom:0;text-align:right;text-decoration:none"">
                                                                    <img src=""images/logo-footer.jpg"" width=""129"" alt=""alt_text"" border=""0"" style=""border:0;display:inline-block;padding:0;max-width:129px;"">
                                                                </a> 
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <!--[if mso]>
                                    </td>
                                    </tr>
                                    </table>
                                    <![endif]-->
                                    </td>
                                </tr>
                            </table>
                            <!--[if mso]>
                            </td>
                            </tr>
                            </table>
                            <![endif]-->
                            </td>
                        </tr>
                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                    <br />
                </div>
            </center>
        </td>
    </tr>
    </table>
</body>
</html>";

        private readonly string ValidationBody = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
	<meta charset=""utf-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
	<title>Unicap</title>
    <!--[if mso]>
        <style>* {font-family: sans-serif !important;}</style>
    <![endif]-->
    <!--[if !mso]><!-->
        <!-- insert web font reference, eg: <link href='https://fonts.googleapis.com/css?family=Roboto:400,700' rel='stylesheet' type='text/css'> -->
    <!--<![endif]-->
    <style type=""text/css"">
        html,body{Margin:0 !important;padding:0 !important;height:100% !important;width:100% !important}
        *{-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%}
        div[style*=""margin: 16px 0""]{margin:0 !important}
        table,td{mso-table-lspace:0pt !important;mso-table-rspace:0pt !important}
        table{border-spacing:0 !important;border-collapse:collapse !important;table-layout:fixed !important;Margin:0 auto !important}
        table table table{table-layout:auto}
        img{-ms-interpolation-mode:bicubic}
        .yshortcuts a{border-bottom:none !important}
        .mobile-link--footer a,a[x-apple-data-detectors]{color:inherit !important;text-decoration:underline !important}
        @media screen and (max-width:600px) {
            .fluid,.fluid-centered{width:100% !important;max-width:100% !important;height:auto !important;Margin-left:auto !important;Margin-right:auto !important}
            .fluid-centered{Margin-left:auto !important;Margin-right:auto !important}
            .stack-column,.stack-column-center{display:block !important;width:100% !important;max-width:100% !important;direction:ltr !important}
            .stack-column-center{text-align:center !important}
            .center-on-narrow{text-align:center !important;display:block !important;Margin-left:auto !important;Margin-right:auto !important;float:none !important}
            table.center-on-narrow{display:inline-block !important}
            *[class=""mobileOff""]{display:none !important;width:0px !important}
        }
    </style>
</head>
<body width=""100%"" bgcolor=""#eeeeee"" style=""background-color:#eeeeee; Margin: 0"">
<table id=""maintable"" cellpadding=""0"" cellspacing=""0"" border=""0"" height=""100%"" width=""100%"" bgcolor=""#eeeeee"" style=""border-collapse:collapse;max-width: 600px;"">
    <tr>
        <td valign=""top"">
            <center style=""width:100%"">
                <div style=""max-width: 600px;"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" align=""center"">
                    <tr>
                    <td>
                    <![endif]-->    
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width: 600px"">
        	            <tr>
        					<td style=""background-color:#ffffff; border-top: solid 4px #427147; padding: 35px 0; border-bottom: solid 1px #f1f1f1;"">
                                <a href=""javascript:void"" target=""_blank"" style=""display:block;margin:0;outline:none;padding-bottom:0;text-align:center;text-decoration:none"">
                                    <img src=""http://hmlrebens.iasdigitalgroup.com/unicap/images/logo.jpg"" alt=""Clube Unicap"" border=""0"" style=""border:0;display:block;margin:0 auto;padding:0;"">
                                </a>
        					</td>
        	            </tr>
                    </table>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px"" bgcolor=""#ffffff"">
                        <tr>
                            <td style=""padding: 45px 0"" class=""stack-column"">
                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""Margin: auto"">
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 25px 0 0 0;"">
                                            <p style=""display:inline-block;text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #427147; margin: 0;padding: 0 20px; text-transform: uppercase;font-weight: bold;"">BEM-VINDO</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 0 0 0 0;""> 
                                            <p style=""display:inline-block;text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;text-transform: uppercase;font-weight: bold;"">AO CLUBE DO UNICAP!</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:center; padding: 25px 0 25px 0;"">
                                            <a href=""##LINK##"" target=""_blank"" style=""display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;font-size: 14px;padding: 15px 50px;font-family:verdana, arial, Helvetica; color: #ffffff;background-color:#427147;border-radius:50px;"">
                                                ATIVAR E-MAIL
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px"" bgcolor=""#ffffff"">
                        <tr>
                            <td style=""padding: 50px 0; border-top: solid 1px #f1f1f1;"" class=""stack-column-center"">
                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""left"" width=""100%"" style=""Margin: auto"">
                                    <tr>
                                        <td class=""stack-column"" style=""text-align:left;padding-bottom: 50px"">
                                            <p style=""display:block;padding-bottom:0;text-align:left; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666;  padding: 0 20px"">Estaremos atentos às suas atividades e buscaremos novidades de acordo com o seu perfil. Fique ligado! A vontade é o limite, e ela é infinita!</p>

                                            <p style=""display:block;padding-bottom:0;text-align:left; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666;  padding: 0 20px"">Vamos lhe oferecer o que há de melhor!!!</p>

                                            <p style=""display:block;padding-bottom:0;text-align:left; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666;  padding: 0 20px"">Em caso de dúvidas nos contate através do e-mail: <br><a href=""mailto:contato@clubunicap.com.br"" target=""_blank"" style=""margin:0;outline:none;padding-bottom:0;text-align:center;color: #427147;"">contato@clubunicap.com.br</a></p>

                                            <p style=""display:block;padding-bottom:0;text-align:left; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666;  padding: 0 20px"">Até mais,<br>
                                            CLUBE DO MANTENEDOR - <a href=""http://www.mantenedor.rebens.com.br"" target=""_blank"" style=""margin:0;outline:none;padding-bottom:0;text-align:center;color: #427147;"">www.mantenedor.rebens.com.br</a></p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table bgcolor="""" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width:600px;border-top: solid 1px #f1f1f1;"">
                        <tr>
                            <td style=""border-bottom: solid 4px #427147"" bgcolor=""#ffffff"" align=""center"" height=""100%"" valign=""top"" width=""100%"">
                            <!--[if mso]>
                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" width=""560"">
                            <tr>
                            <td align=""center"" valign=""top"" width=""600"">
                            <![endif]-->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" style=""max-width:600px;"">
                                <tr>
                                    <td align=""center"" valign=""top"" style=""font-size:0;padding:20px; "">
                                    <!--[if mso]>
                                    <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" width=""600"">
                                    <tr>
                                    <td align=""left"" valign=""top"" width=""280"">
                                    <![endif]-->
                                    <div style=""display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;"" class=""stack-column"">
                                        
                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                            <tr>
                                                <td>
                                                    <table class=""stack-column"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""font-size:14px;text-align: left;"">
                                                        <tr>
                                                            <td class=""stack-column"" bgcolor=""#ffffff"" style=""text-align:left;"">
                                                                <p style=""display:block;padding-bottom:0;text-align:left; font-size: 10px; font-family: arial, Helvetica; color: #666666;""><strong>Rebens - CNPJ: 21.296.610/0001-54</strong><br>
                                                                    Rua Haddock Lobo, 1307 - Cj 82 <br>Cerqueira César - São Paulo/SP - 04551-000</p>
                                                            </td>
                                                        </tr> 
                                                    </table>
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                        
                                    </div>
                                    <!--[if mso]>
                                    </td>
                                    <td align=""left"" valign=""top"" width=""280"">
                                    <![endif]-->
                                    <div style=""display:inline-block;min-width:200px;max-width:280px;vertical-align:top;width:100%;"" class=""stack-column"">
                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                            <tr>
                                                <td>
                                                    <table class=""stack-column"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""font-size:14px;text-align: right;"">
                                                        <tr>
                                                            <td class=""stack-column"" bgcolor=""#ffffff"" style=""text-align:right;"">
                                                               <a href=""javascript:void"" target=""_blank"" style=""display:block;margin:0;outline:none;padding-bottom:0;text-align:right;text-decoration:none"">
                                                                    <img src=""images/logo-footer.jpg"" width=""129"" alt=""alt_text"" border=""0"" style=""border:0;display:inline-block;padding:0;max-width:129px;"">
                                                                </a> 
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <!--[if mso]>
                                    </td>
                                    </tr>
                                    </table>
                                    <![endif]-->
                                    </td>
                                </tr>
                            </table>
                            <!--[if mso]>
                            </td>
                            </tr>
                            </table>
                            <![endif]-->
                            </td>
                        </tr>
                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                    <br />
                </div>
            </center>
        </td>
    </tr>
    </table>
</body>
</html>";
        #endregion  MailTemplate
    }
}
