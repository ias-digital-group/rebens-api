using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ias.Rebens.Integration
{
    public class CouponToolsHelper
    {
        const string URL = "https://api4coupons.com/v3/";
        const string CLIENT_ID = "2292342788435322989231952429692";
        const string CLIENT_SECRET = "QuWLSvP2GKM9QYGdkXUU8f4khEJpM9b";

        public bool CreateSingle(Customer customer, Coupon coupon, out string error)
        {
            bool ret = false;
            var postData = "campaign=cam_1009976";// + coupon.Campaign; 
            postData += "&firstname=" + customer.Name.Trim().Split(" ")[0];
            postData += "&email=" + customer.Email;
            postData += "&customid=" + coupon.Id;
            postData += "&customvalcode=" + coupon.ValidationCode;
            var data = Encoding.ASCII.GetBytes(postData);

            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create(URL + "singleuse/create") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Timeout = 30000;
            request.Headers.Add("X-Client-id", CLIENT_ID);
            request.Headers.Add("X-Client-secret", CLIENT_SECRET);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
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

                    if (jObj["status"]["status"].ToString() == "OK")
                    {
                        coupon.SingleUseUrl = jObj["single_use_url"].ToString();
                        coupon.SingleUseCode = jObj["single_use_code"].ToString();
                        coupon.WidgetValidationCode = jObj["widget_validation_code"].ToString();
                        ret = true;
                        error = null;
                    }
                    else
                        error = "Ocorreu um erro ao tentar criar o cupom do coupontools";
                }
                else
                    error = "Ocorreu um erro ao tentar criar o cupom do coupontools";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return ret;
        }

        public List<CouponCampaign> ListCampaigns(out string error)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create(URL + "coupon/list") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 30000;
            request.Headers.Add("X-Client-id", CLIENT_ID);
            request.Headers.Add("X-Client-secret", CLIENT_SECRET);

            var list = new List<CouponCampaign>();
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

                    if (jObj["status"]["status"].ToString() == "OK")
                    {
                        var tmp = jObj["coupon_info"].Children();
                        foreach (var item in tmp)
                        {
                            list.Add(new CouponCampaign()
                            {
                                CampaignId = item["ID"].ToString(),
                                Code = item["code"].ToString(),
                                Created = DateTime.UtcNow,
                                Modified = DateTime.UtcNow,
                                Name = item["name"].ToString(),
                                Title = item["title"].ToString(),
                                Status = item["status"].ToString(),
                                Url = item["url"].ToString()
                            });
                        }
                        error = null;
                    }
                    else
                        error = "Ocorreu um erro ao tentar listar as campanhas do coupontools";
                }
                else
                    error = "Ocorreu um erro ao tentar listar as campanhas do coupontools";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return list;
        }

        public bool GetCouponData(Coupon coupon, out string error)
        {
            bool ret = false;
            var postData = "couponsession=" + coupon.SingleUseCode;
            var data = Encoding.ASCII.GetBytes(postData);

            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create(URL + "couponsession/data") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Timeout = 30000;
            request.Headers.Add("X-Client-id", CLIENT_ID);
            request.Headers.Add("X-Client-secret", CLIENT_SECRET);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
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

                    if (jObj["status"]["status"].ToString() == "OK")
                    {
                        coupon.SequenceId = Convert.ToInt64(jObj["sequenceid"].ToString());
                        if(jObj["data"] != null && jObj["data"]["status"] != null)
                        {
                            if(jObj["data"]["status"]["open_date_utc"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["open_date_utc"].ToString()))
                            {
                                if (DateTime.TryParse(jObj["data"]["status"]["open_date_utc"].ToString(), out DateTime dt))
                                    coupon.OpenDate = dt;
                            }
                            if (jObj["data"]["status"]["played_date_utc"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["played_date_utc"].ToString()))
                            {
                                if (DateTime.TryParse(jObj["data"]["status"]["played_date_utc"].ToString(), out DateTime dt))
                                    coupon.PlayedDate = dt;
                            }
                            if (jObj["data"]["status"]["claim_date_utc"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["claim_date_utc"].ToString()))
                            {
                                if (DateTime.TryParse(jObj["data"]["status"]["claim_date_utc"].ToString(), out DateTime dt))
                                    coupon.ClaimDate = dt;
                            }
                            if (jObj["data"]["status"]["validation_date_utc"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["validation_date_utc"].ToString()))
                            {
                                if (DateTime.TryParse(jObj["data"]["status"]["validation_date_utc"].ToString(), out DateTime dt))
                                    coupon.ValidationDate = dt;
                            }
                            if (jObj["data"]["status"]["voided_date_utc"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["voided_date_utc"].ToString()))
                            {
                                if (DateTime.TryParse(jObj["data"]["status"]["voided_date_utc"].ToString(), out DateTime dt))
                                    coupon.VoidedDate = dt;
                            }
                            if (jObj["data"]["status"]["locked"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["locked"].ToString()))
                                coupon.Locked = jObj["data"]["status"]["locked"].ToString() == "1";
                            if (jObj["data"]["status"]["claimtype"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["claimtype"].ToString()))
                                coupon.ClaimType = jObj["data"]["status"]["claimtype"].ToString();
                            if (jObj["data"]["status"]["validation_value"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["validation_value"].ToString()))
                                coupon.ValidationValue = jObj["data"]["status"]["validation_value"].ToString();
                            if (jObj["data"]["status"]["value"] != null && !string.IsNullOrEmpty(jObj["data"]["status"]["value"].ToString()))
                                coupon.Value = jObj["data"]["status"]["value"].ToString();
                        }

                        ret = true;
                        error = null;
                    }
                    else
                        error = "Ocorreu um erro ao tentar ler o cupom do coupontools";
                }
                else
                    error = "Ocorreu um erro ao tentar ler o cupom do coupontools";
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return ret;
        }
    }
}
