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
    }
}
