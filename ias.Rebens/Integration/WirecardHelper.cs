using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ias.Rebens.Integration
{
    public class WirecardHelper
    {
        // Sandbox
        //private const string TOKEN = "TEhFUUxVRDdPOE9QT0pWU0U1SkZFRzVJRjhXWU5HMlM6TkZWSzlVRUJHVEJJSVUwWE9KMkdFWUc3TUU5S0xaQUNDWkJCUEhEUQ==";
        //private const string SIGNATURE_URL = "https://sandbox.moip.com.br/assinaturas/v1/";
        //private const string PAYMENT_URL = "https://sandbox.moip.com.br/v2/";
        // Production
        private const string TOKEN = "MkdQN1pHWEk3QTBYUkJZT0ZISjlUSVg1Qk1HQ0xKTUs6UUgyT0JGQVBaNjFJSEdMVU9TVzg3WURXRjNYTVpFNU9HMEFOQlE2VQ==";
        private const string SIGNATURE_URL = "https://api.moip.com.br/assinaturas/v1/";
        private const string PAYMENT_URL = "https://api.moip.com.br/v2/";

        #region Order
        public bool CheckPaymentStatus(WirecardPayment payment)
        {
            bool ret = false;
            try
            {
                string resp = CallApi("GET", $"payments/{payment.Id}", null, PAYMENT_URL);
                if (resp != null) 
                {
                    var jObj = JObject.Parse(resp);
                    payment.Status = jObj["status"].ToString();
                    payment.Modified = DateTime.UtcNow;
                    ret = true;
                }
            }
            catch { }

            return ret;
        }

        public bool CheckOrderStatus(Order order)
        {
            bool ret = false;
            try
            {
                string resp = CallApi("GET", $"orders/{order.WirecardId}", null, PAYMENT_URL);
                if (resp != null)
                {
                    var jObj = JObject.Parse(resp);
                    order.Status = jObj["status"].ToString();
                    order.Modified = DateTime.UtcNow;
                    ret = true;
                }
            }
            catch { }

            return ret;
        }
        #endregion Order

        #region Signature
        public List<MoipSignature> ListSubscriptions()
        {
            var list = new List<MoipSignature>();
            try
            {
                string ret = CallApi("GET", "subscriptions", null, SIGNATURE_URL);
                var jObj = JObject.Parse(ret);

                var objList = jObj["subscriptions"].Children();
                foreach (var item in objList)
                {
                    var sub = new MoipSignature()
                    {
                        Code = item["id"].ToString(),
                        PlanCode = item["plan"]["code"].ToString(),
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        PaymentMethod = item["payment_method"].ToString(),
                        Status = item["status"].ToString()
                    };
                    if (int.TryParse(item["amount"].ToString(), out int amount))
                        sub.Amount = (decimal)(amount / 100);
                    sub.CreationDate = new DateTime(int.Parse(item["creation_date"]["year"].ToString()), 
                                            int.Parse(item["creation_date"]["month"].ToString()), 
                                            int.Parse(item["creation_date"]["day"].ToString()), 
                                            int.Parse(item["creation_date"]["hour"].ToString()), 
                                            int.Parse(item["creation_date"]["minute"].ToString()), 
                                            int.Parse(item["creation_date"]["second"].ToString()));
                    if(item["next_invoice_date"] != null)
                        sub.NextInvoiceDate = new DateTime(int.Parse(item["next_invoice_date"]["year"].ToString()), 
                                                int.Parse(item["next_invoice_date"]["month"].ToString()), 
                                                int.Parse(item["next_invoice_date"]["day"].ToString()));

                    var invoice = new MoipInvoice()
                    {
                        Id = int.Parse(item["invoice"]["id"].ToString()),
                        Created = DateTime.Now,
                        IdStatus = int.Parse(item["invoice"]["status"]["code"].ToString()),
                        Modified = DateTime.Now,
                        Status = item["invoice"]["status"]["description"].ToString()
                    };
                    if (item["invoice"]["amount"] != null && int.TryParse(item["invoice"]["amount"].ToString(), out int invoiceAmount))
                        invoice.Amount = (decimal)(invoiceAmount / 100);


                    var payment = new MoipPayment()
                    {
                        IdMoipInvoice = invoice.Id,
                        MoipId = long.Parse(item["code"].ToString()),
                        IdStatus = invoice.IdStatus,
                        Status = invoice.Status,
                        PaymentMethod = sub.PaymentMethod,
                        Brand = item["customer"]["billing_info"]["credit_card"]["brand"].ToString(),
                        HolderName = item["customer"]["billing_info"]["credit_card"]["holder_name"].ToString(),
                        FirstSixDigits = item["customer"]["billing_info"]["credit_card"]["first_six_digits"].ToString(),
                        LastFourDigits = item["customer"]["billing_info"]["credit_card"]["last_four_digits"].ToString(),
                        Vault = item["customer"]["billing_info"]["credit_card"]["vault"].ToString(),
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        Amount = sub.Amount
                    };

                    invoice.Payments.Add(payment);
                    sub.Invoices.Add(invoice);
                    list.Add(sub);
                }
            }
            catch { }

            return list;
        }
        #endregion Signature

        #region CallApi
        private string CallApi(string method, string uri, string serializedObject, string apiUrl)
        {
            string ret = null;
            var timestamp = DateTime.UtcNow;
            string nonce = Helper.SecurityHelper.GenerateNonce(30);
            string stringToSign = method + uri + timestamp.ToString("r") + nonce;
            string autorization = $"Basic {TOKEN}";

            string apiroot = apiUrl + uri;
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", autorization);
            request.Date = timestamp;

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
        #endregion CallApi
    }
}
