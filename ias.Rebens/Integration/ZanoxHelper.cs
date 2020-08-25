using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if (jObj["saleItems"] != null && jObj["saleItems"]["saleItem"] != null)
                {
                    var objList = jObj["saleItems"]["saleItem"].Children();

                    foreach (var item in objList)
                    {
                        var sale = new ZanoxSale
                        {
                            ZanoxId = item["@id"].ToString(),
                            ReviewState = item["reviewState"].ToString(),
                            ClickId = long.Parse(item["clickId"].ToString()),
                            ClickInId = long.Parse(item["clickInId"].ToString()),
                            Amount = decimal.Parse(item["amount"].ToString()),
                            Commission = decimal.Parse(item["commission"].ToString()),
                            Currency = item["currency"].ToString(),
                            Created = DateTime.Now,
                            Modified = DateTime.Now
                        };

                        if (item["trackingDate"] != null)
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

                        if (item["adspace"] != null)
                        {
                            if (item["adspace"]["@id"] != null)
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
                        if (item["reviewNote"] != null)
                            sale.ReviewNote = item["reviewNote"].ToString();
                        if (item["gpps"] != null)
                        {
                            sale.Gpps = item["gpps"].ToString();
                            if (item["gpps"]["gpp"] != null && item["gpps"]["gpp"] != null)
                            {
                                try
                                {
                                    if (item["gpps"]["gpp"]["@id"] != null)
                                        if (item["gpps"]["gpp"]["@id"].ToString() == "zpar0")
                                            sale.Zpar = item["gpps"]["gpp"]["$"].ToString();
                                }
                                catch { }

                                try
                                {
                                    var childrens = item["gpps"]["gpp"].Children();
                                    foreach (var child in childrens)
                                    {
                                        if (child["@id"] != null && child["@id"].ToString() == "zpar0")
                                            sale.Zpar = child["$"].ToString();
                                    }
                                }
                                catch { }
                            }
                        }

                        sale.Status = (int)Enums.ZanoxStatus.pendent;

                        list.Add(sale);
                    }
                }

                error = null;
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }
            return list;
        }

        public List<ZanoxProgram> GetPrograms(out string error)
        {
            List<ZanoxProgram> ret = null;
            try
            {
                var result = CallPublicApi("GET", "/programs", null);
                var jObj = JObject.Parse(result);
                if (jObj["programItems"] != null && jObj["programItems"]["programItem"] != null)
                {
                    ret = new List<ZanoxProgram>();
                    var objList = jObj["programItems"]["programItem"].Children();
                    foreach (var item in objList)
                    {
                        var program = new ZanoxProgram()
                        {
                            Id = item["@id"].Value<int>(),
                            Name = item["name"].ToString(),
                            Created = DateTime.UtcNow,
                            Modified = DateTime.UtcNow
                        };

                        if (item["adrank"] != null)
                            program.AdRank = item["adrank"].Value<decimal>();
                        if (item["description"] != null)
                            program.Description = item["description"].ToString();
                        if (item["descriptionLocal"] != null)
                            program.LocalDescription = item["descriptionLocal"].ToString();
                        if (item["url"] != null)
                            program.Url = item["url"].ToString();
                        if (item["image"] != null)
                            program.Image = item["image"].ToString();
                        if (item["currency"] != null)
                            program.Currency = item["currency"].ToString();
                        if (item["status"] != null)
                            program.Status = item["status"].ToString();
                        if (item["terms"] != null)
                            program.Terms = item["terms"].ToString();
                        if (item["startDate"] != null)
                        {
                            try
                            {
                                program.StartDate = item["startDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["startDate"].ToString(), out DateTime dt))
                                    program.StartDate = dt;
                            }
                        }
                        program.Active = program.Status == "active";

                        ret.Add(program);
                    }
                    error = null;
                }
                else
                    error = "Programas não encontrados!";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return ret;
        }

        public ZanoxProgram GetProgram(int id, out string error)
        {
            ZanoxProgram ret = null;
            try
            {
                var result = CallPublicApi("GET", $"/programs/program/{id}", null);
                var jObj = JObject.Parse(result);
                if (jObj["programItem"] != null)
                {
                    var item = jObj["programItem"].Children().FirstOrDefault();
                    if (item != null)
                    {
                        ret = new ZanoxProgram()
                        {
                            Id = item["@id"].Value<int>(),
                            Name = item["name"].ToString(),
                            Created = DateTime.UtcNow,
                            Modified = DateTime.UtcNow
                        };

                        if (item["adrank"] != null)
                            ret.AdRank = item["adrank"].Value<decimal>();
                        if (item["description"] != null)
                            ret.Description = item["description"].ToString();
                        if (item["descriptionLocal"] != null)
                            ret.LocalDescription = item["descriptionLocal"].ToString();
                        if (item["url"] != null)
                            ret.Url = item["url"].ToString();
                        if (item["image"] != null)
                            ret.Image = item["image"].ToString();
                        if (item["currency"] != null)
                            ret.Currency = item["currency"].ToString();
                        if (item["status"] != null)
                            ret.Status = item["status"].ToString();
                        if (item["terms"] != null)
                            ret.Terms = item["terms"].ToString();
                        if (item["startDate"] != null)
                        {
                            try
                            {
                                ret.StartDate = item["startDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["startDate"].ToString(), out DateTime dt))
                                    ret.StartDate = dt;
                            }
                        }
                        ret.Active = ret.Status == "active";
                    }
                    error = null;
                }
                else
                    error = "Programa não encontrado!";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return ret;
        }

        public List<ZanoxIncentive> GetIncentives(out string error)
        {
            List<ZanoxIncentive> ret = null;
            try
            {
                var result = CallPublicApi("GET", "/incentives?region=BR", null);
                var jObj = JObject.Parse(result);
                if (jObj["incentiveItems"] != null && jObj["incentiveItems"]["incentiveItem"] != null)
                {
                    ret = new List<ZanoxIncentive>();
                    var objList = jObj["incentiveItems"]["incentiveItem"].Children();
                    foreach (var item in objList)
                    {
                        var incentive = new ZanoxIncentive()
                        {
                            Id = item["@id"].Value<int>(),
                            Name = item["name"].ToString(),
                            IdProgram = item["program"]["@id"].Value<int>(),
                            Created = DateTime.UtcNow,
                            Modified = DateTime.UtcNow,
                            Removed = false,
                            Active = true
                        };

                        if (item["incentiveType"] != null)
                            incentive.Type = item["incentiveType"].ToString();
                        if (item["info4publisher"] != null)
                            incentive.PublisherInfo = item["info4publisher"].ToString();
                        if (item["info4customer"] != null)
                            incentive.CustomerInfo = item["info4customer"].ToString();
                        if (item["restrictions"] != null)
                            incentive.Restriction = item["restrictions"].ToString();
                        if (item["couponCode"] != null)
                            incentive.Code = item["couponCode"].ToString();
                        if (item["currency"] != null)
                            incentive.Currency = item["currency"].ToString();
                        if (item["total"] != null)
                            incentive.Amount = item["total"].Value<decimal>();
                        if (item["createDate"] != null)
                        {
                            try
                            {
                                incentive.ZanoxCreated = item["createDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["createDate"].ToString(), out DateTime dt))
                                    incentive.ZanoxCreated = dt;
                            }
                        }
                        if (item["modifiedDate"] != null)
                        {
                            try
                            {
                                incentive.ZanoxModified = item["modifiedDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["modifiedDate"].ToString(), out DateTime dt))
                                    incentive.ZanoxModified = dt;
                            }
                        }
                        if (item["startDate"] != null)
                        {
                            try
                            {
                                incentive.Start = item["startDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["startDate"].ToString(), out DateTime dt))
                                    incentive.Start = dt;
                            }
                        }
                        if (item["endDate"] != null)
                        {
                            try
                            {
                                incentive.End = item["endDate"].Value<DateTime>();
                            }
                            catch
                            {
                                if (DateTime.TryParse(item["endDate"].ToString(), out DateTime dt))
                                    incentive.End = dt;
                            }
                        }
                        if(item["admedia"] != null && item["admedia"]["admediumItem"] != null 
                            && item["admedia"]["admediumItem"]["trackingLinks"] != null
                            && item["admedia"]["admediumItem"]["trackingLinks"].ToString() != ""
                            && item["admedia"]["admediumItem"]["trackingLinks"]["trackingLink"] != null)
                        {
                            var links = item["admedia"]["admediumItem"]["trackingLinks"]["trackingLink"].Children();
                            var link = links.FirstOrDefault();
                            if (link != null)
                                incentive.Url = link["ppc"].ToString();
                        }

                            ret.Add(incentive);
                    }
                }
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return ret;
        }

        public string CallPublicApi(string method, string uri, string serializedObject)
        {
            string ret = null;
            string apiroot = URL + uri + $"{(uri.Contains("?") ? "&" : "?")}connectid={CONNECT_ID}";
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            
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

        public string CallApi(string method, string uri, string serializedObject)
        {
            string ret = null;
            var timestamp = DateTime.UtcNow;
            string nonce = Helper.SecurityHelper.GenerateNonce(30);
            string stringToSign = method + uri + timestamp.ToString("r") + nonce;
            string signature = Helper.SecurityHelper.HMACSHA1(SECRTE_KEY, stringToSign);
            string autorization = $"ZXWS {CONNECT_ID}:{signature}";

            string apiroot = URL + uri;
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", autorization);
            request.Date = timestamp;
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
