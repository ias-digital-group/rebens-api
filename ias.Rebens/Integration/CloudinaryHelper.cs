using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ias.Rebens.Integration
{
    public class CloudinaryHelper
    {
        const string API_KEY = "873394135826967";
        const string API_SECRET = "4Ux4no-NdhJIKkXihQaxstvMzz4";
        const string URL = "https://api.cloudinary.com/v1_1/rebens/image/upload";
        const string ENVOIREMENT_VARIABLE = "CLOUDINARY_URL=cloudinary://873394135826967:4Ux4no-NdhJIKkXihQaxstvMzz4@rebens";

        public Models.CloudinaryModel UploadFile(string filePath, string folder)
        {
            var resultModel = new Models.CloudinaryModel();

            var encoding = new ASCIIEncoding();
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            //string folder = "rebens";
            var temp = "folder=rebens&timestamp=" + timestamp + API_SECRET;
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(temp));
            string signature = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());

            var fileBytes = File.ReadAllBytes(filePath);
            string encodedFile = "data:image/png;base64," + Convert.ToBase64String(fileBytes);

            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Timeout = 30000;

            Dictionary<string, Object> data = new Dictionary<string, Object>();
            data.Add("api_key", API_KEY);
            data.Add("timestamp", timestamp);
            data.Add("signature", signature);
            data.Add("file", encodedFile);
            data.Add("folder", folder);

            string content = JsonConvert.SerializeObject(data);

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
                    int successRead = 0;
                    var ms = new MemoryStream();
                    do
                    {
                        successRead = stream.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, successRead);
                    } while (successRead > 0);

                    var responseString = encoding.GetString(ms.ToArray());
                    resultModel = JsonConvert.DeserializeObject<Models.CloudinaryModel>(responseString);
                    resultModel.Status = true;
                }
                else
                {
                    var stream = response.GetResponseStream() as Stream;
                    byte[] buffer = new byte[32 * 1024];
                    int successRead = 0;
                    var ms = new MemoryStream();
                    do
                    {
                        successRead = stream.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, successRead);
                    } while (successRead > 0);
                    var responseString = encoding.GetString(ms.ToArray());
                    var jObj = JObject.Parse(responseString);
                    resultModel.Status = false;
                    resultModel.Message = jObj["error"]["message"].ToString();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    resultModel.Status = false;
                    resultModel.Message = "Request failed";
                }
            }

            return resultModel;
        }
    }
}
