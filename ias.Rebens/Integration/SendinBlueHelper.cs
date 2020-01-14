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
        const string API_KEY_V3 = "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";

        public Models.SendinBlueModel Send(Dictionary<string, string> listDestinataries, string fromEmail, string fromName, string subject, string body)
        {
            var resultModel = new Models.SendinBlueModel();

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            //Dictionary<string, string> to = new Dictionary<string, string>();
            //to.Add(toEmail, toName);
            List<string> from_name = new List<string>();
            from_name.Add(fromEmail);
            from_name.Add(fromName);
            //List<string> attachment = new List<string>();
            //attachment.Add("https://domain.com/path-to-file/filename1.pdf");
            //attachment.Add("https://domain.com/path-to-file/filename2.jpg");

            data.Add("to", listDestinataries);
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

        public bool CreateList(Operation operation, out int listId, out string error)
        {
            var ret = false;

            Dictionary<string, Object> data = new Dictionary<string, Object>();

            data.Add("name", operation.Title);
            data.Add("folderId", 40);

            string content = JsonConvert.SerializeObject(data);
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create("https://api.sendinblue.com/v3/contacts/lists") as HttpWebRequest;

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
                if (response.StatusCode == HttpStatusCode.OK)
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

                    listId = Convert.ToInt32(jObj["id"]);
                    error = null;
                    ret = true;
                }
                else
                {
                    listId = 0;
                    try
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
                        error = jObj["message"].ToString();
                    }
                    catch
                    {
                        error = "Ocorreu um erro ao tentar criar a lista no sendingblue";
                    }
                }
            }
            catch
            {
                listId = 0;
                error = "Ocorreu um erro ao tentar criar a lista no sendingblue";
            }
            return ret;
        }

        public bool CreateContact(Customer customer, Operation operation, out string error)
        {
            var ret = false;

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            Dictionary<string, Object> attributes = new Dictionary<string, Object>();

            data.Add("email", customer.Email);
            attributes.Add("CPF", customer.Cpf);
            attributes.Add("NAME", customer.Name);
            attributes.Add("GENERO", customer.Gender);
            if(customer.Birthday.HasValue)
                attributes.Add("DATA_DE_NASCIMENTO", customer.Birthday.Value.ToString("dd/MM/yyyy"));
            attributes.Add("UF", customer.Address.State);
            attributes.Add("ID_OPERACAO", operation.Id);
            attributes.Add("CIDADE", customer.Address.City);
            attributes.Add("SMS", customer.Cellphone);
            data.Add("attributes", attributes);
            data.Add("emailBlacklisted", false);
            data.Add("smsBlacklisted", false);
            data.Add("updateEnabled", true);
            int[] aIds = { 41 };
            data.Add("listIds", aIds);

            string content = JsonConvert.SerializeObject(data);
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create("https://api.sendinblue.com/v3/contacts/lists") as HttpWebRequest;

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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    error = null;
                    ret = true;
                }
                else
                {
                    try
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
                        error = jObj["message"].ToString();
                    }
                    catch
                    {
                        error = "Ocorreu um erro ao tentar criar a lista no sendingblue";
                    }
                }
            }
            catch
            {
                error = "Ocorreu um erro ao tentar criar a lista no sendingblue";
            }
            return ret;
        }
    }
}
