using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ias.Rebens.Integration
{
    public class ZanoxHelper
    {
        const string CONNECT_ID = "D9D188F49CB8521B5157";
        const string SECRTE_KEY = "6a741684831e4C+cacc8cf147aD893/30a9dc94A";
        const string URL = "https://api.zanox.com/json/2011-03-01";

        public List<ZanoxSale> UpdateZanoxSales(DateTime date, out string error)
        {
            List<ZanoxSale> list = new List<ZanoxSale>();
            var url = "/reports/sales/date/" + date.ToString("yyyy-MM-dd");

            try
            {
                var ret = CallApi("GET", url, null);

                var jObj = JObject.Parse(ret);
                var objList = jObj["saleItems"]["saleItem"].Children();

                foreach (var item in objList)
                {
                    var sale = new ZanoxSale();
                    sale.ZanoxId = item["@id"].ToString();
                    sale.ReviewState = item["reviewState"].ToString();
                    if(item["trackingDate"] != null)
                    {
                        if (DateTime.TryParse(item["trackingDate"].ToString(), out DateTime dt))
                            sale.TrackingDate = dt;
                    }
                    if (item["modifiedDate"] != null)
                    {
                        if (DateTime.TryParse(item["modifiedDate"].ToString(), out DateTime dt))
                            sale.ModifiedDate = dt;
                    }
                    if (item["clickDate"] != null)
                    {
                        if (DateTime.TryParse(item["clickDate"].ToString(), out DateTime dt))
                            sale.ClickDate = dt;
                    }
                    sale.ClickId = long.Parse(item["clickId"].ToString());
                    sale.ClickInId = long.Parse(item["clickInId"].ToString());
                    sale.Amount = decimal.Parse(item["amount"].ToString());
                    sale.Commission = decimal.Parse(item["commission"].ToString());
                    sale.Currency = item["currency"].ToString();
                    sale.Created = sale.Modified = DateTime.Now;

                    if(item["adspace"] != null)
                    {
                        if(item["adspace"]["@id"] != null)
                        {
                            if (int.TryParse(item["adspace"]["@id"].ToString(), out int tmpId))
                                sale.AdspaceId = tmpId;
                        }
                        if (item["adspace"]["$"] != null)
                            sale.AdspaceValue = item["adspace"]["$"].ToString();
                    }
                    if (item["admedium"] != null)
                    {
                        if (item["admedium"]["@id"] != null)
                        {
                            if (int.TryParse(item["admedium"]["@id"].ToString(), out int tmpId))
                                sale.AdmediumId = tmpId;
                        }
                        if (item["admedium"]["$"] != null)
                            sale.AdmediumValue = item["admedium"]["$"].ToString();
                    }
                    if (item["program"] != null)
                    {
                        if (item["program"]["@id"] != null)
                        {
                            if (int.TryParse(item["program"]["@id"].ToString(), out int tmpId))
                                sale.ProgramId = tmpId;
                        }
                        if (item["program"]["$"] != null)
                            sale.ProgramValue = item["program"]["$"].ToString();
                    }
                    if(item["reviewNote"] != null)
                        sale.ReviewNote = item["reviewNote"].ToString();
                    if (item["gpps"] != null)
                    {
                        sale.Gpps = item["gpps"].ToString();
                        if (item["gpps"]["gpp"] != null && item["gpps"]["gpp"] != null && item["gpps"]["gpp"]["@id"] != null)
                        {
                            if (item["gpps"]["gpp"]["@id"].ToString() == "zpar0")
                                sale.Zpar = item["gpps"]["gpp"]["$"].ToString();
                        }
                    }
                    sale.Status = (int)Enums.ZanoxStatus.pendent;

                    list.Add(sale);
                }

                error = null;
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
            return list;
        }

        public string CallApi(string method, string uri, string serializedObject)
        {
            string ret = null;
            string timestamp = DateTime.UtcNow.ToString("r");
            string nonce = Helper.SecurityHelper.GenerateNonce(30);
            string stringToSign = method + uri + timestamp + nonce;
            string signature = Helper.SecurityHelper.HMACSHA1(SECRTE_KEY, stringToSign);
            string autorization = $"ZXWS {CONNECT_ID}:{signature}";

            string apiroot = URL + uri;
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", autorization);
            request.Headers.Add("date", timestamp);
            request.Headers.Add("nonce", nonce);

            if (serializedObject != null)
            {
                request.AllowWriteStreamBuffering = false;
                request.SendChunked = false;

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(serializedObject);
                request.ContentLength = bytes.Length;
                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        ret = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
    }
}
