using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using ias.Rebens;
using System.Threading;
using System.Collections.Specialized;
using System.Web;

namespace Test
{
    class Program
    {
        public static string CryptoHash { get { return "p$cTP-7tV/zY"; } }
        public static string SaltKey { get { return "&kxBwk9b'p?5"; } }
        public static string VIKey { get { return "GVL6H]#*d&xteL8a"; } }
        public static int SaltLength { get { return 32; } }
        public static int Pbkdf2Iterations { get { return 5000; } }
        public static int DefaultPasswordLength { get { return 10; } }

        static void Main(string[] args)
        {
            var encoding = Encoding.Unicode;

            string code = "wNOizZqPxtlh9HoQvNcq4A%3d%3d";
            string temp = SimpleDecryption(System.Web.HttpUtility.UrlDecode(code));

            Console.WriteLine(temp);

            //int length = 12;
            //var list = new List<string>();
            //for (int i=0;i<100;i++)
            //{
            //    string tmp = GenerateOTP(length);
            //    Console.WriteLine(tmp);

            //    if (list.Any(l => l == tmp))
            //        Console.WriteLine("############################## ERROR ###############################");
            //    else
            //        list.Add(tmp);

            //    Thread.Sleep(10);
            //}

            Console.WriteLine("DONE");

            Console.ReadLine();
        }

        public static string GenerateOTP(int length)
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }

        public static string SimpleEncryption(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(CryptoHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string SimpleDecryption(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(CryptoHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public static void ListCampaigns()
        {
            string CLIENT_ID = "2292342788435322989231952429692";
            string CLIENT_SECRET = "QuWLSvP2GKM9QYGdkXUU8f4khEJpM9b";

            var postData = "campaign=cam_1010508";
            var data = Encoding.ASCII.GetBytes(postData);

            ASCIIEncoding encoding = new ASCIIEncoding();
            HttpWebRequest request = WebRequest.Create("https://api4coupons.com/v3/campaign/data") as HttpWebRequest;
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
                    var list = new List<Coupon>();
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
                        if(jObj["amount_of_results"].ToString() != "0")
                        {
                            var items = jObj["data"].Children();
                            foreach(var item in items)
                            {
                                if (item["status"] != null && item["status"]["own_validation_code"] != null && item["status"]["own_validation_code"].ToString() == "")
                                    continue;

                                var coupon = new Coupon();
                                coupon.SequenceId = Convert.ToInt64(item["sequenceid"].ToString());
                                if (item["status"]["open_date_utc"] != null && !string.IsNullOrEmpty(item["status"]["open_date_utc"].ToString()))
                                {
                                    if (DateTime.TryParse(item["status"]["open_date_utc"].ToString(), out DateTime dt))
                                        coupon.OpenDate = dt;
                                }
                                if (item["status"]["played_date_utc"] != null && !string.IsNullOrEmpty(item["status"]["played_date_utc"].ToString()))
                                {
                                    if (DateTime.TryParse(item["status"]["played_date_utc"].ToString(), out DateTime dt))
                                        coupon.PlayedDate = dt;
                                }
                                if (item["status"]["claim_date_utc"] != null && !string.IsNullOrEmpty(item["status"]["claim_date_utc"].ToString()))
                                {
                                    if (DateTime.TryParse(item["status"]["claim_date_utc"].ToString(), out DateTime dt))
                                        coupon.ClaimDate = dt;
                                }
                                if (item["status"]["validation_date_utc"] != null && !string.IsNullOrEmpty(item["status"]["validation_date_utc"].ToString()))
                                {
                                    if (DateTime.TryParse(item["status"]["validation_date_utc"].ToString(), out DateTime dt))
                                        coupon.ValidationDate = dt;
                                }
                                if (item["status"]["voided_date_utc"] != null && !string.IsNullOrEmpty(item["status"]["voided_date_utc"].ToString()))
                                {
                                    if (DateTime.TryParse(item["status"]["voided_date_utc"].ToString(), out DateTime dt))
                                        coupon.VoidedDate = dt;
                                }
                                if (item["status"]["locked"] != null && !string.IsNullOrEmpty(item["status"]["locked"].ToString()))
                                    coupon.Locked = item["status"]["locked"].ToString() == "1";
                                if (item["status"]["claimtype"] != null && !string.IsNullOrEmpty(item["status"]["claimtype"].ToString()))
                                    coupon.ClaimType = item["status"]["claimtype"].ToString();
                                if (item["status"]["validation_value"] != null && !string.IsNullOrEmpty(item["status"]["validation_value"].ToString()))
                                    coupon.ValidationValue = item["status"]["validation_value"].ToString();
                                if (item["status"]["value"] != null && !string.IsNullOrEmpty(item["status"]["value"].ToString()))
                                    coupon.Value = item["status"]["value"].ToString();
                                if (item["status"]["own_validation_code"] != null && !string.IsNullOrEmpty(item["status"]["value"].ToString()))
                                    coupon.ValidationCode = item["status"]["own_validation_code"].ToString();
                                if (item["session"] != null && !string.IsNullOrEmpty(item["session"].ToString()))
                                    coupon.SingleUseCode = item["session"].ToString();
                                list.Add(coupon);
                            }
                        }
                    }

                    foreach (var coupon in list) {
                        Console.WriteLine("INSERT INTO Coupon VALUES(30, 1, '{0}', 'Raspadinha Unicap', '{1}', 'https://digicpn.com/p/uzsdqq/{2}', null, {3}, {4}, {5}, '{6}', {7}, '{8}', {9}, 0, '{10}', {11}, 1, GETDATE(), GETDATE() , GETDATE())",                            coupon.ValidationCode, coupon.SingleUseCode, coupon.SingleUseCode,
                            coupon.OpenDate.HasValue ? "'" + coupon.OpenDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                            coupon.PlayedDate.HasValue ? "'" + coupon.PlayedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                            coupon.ClaimDate.HasValue ? "'" + coupon.ClaimDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                            coupon.ClaimType,
                            coupon.ValidationDate.HasValue ? "'" + coupon.ValidationDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                            coupon.ValidationValue,
                            coupon.VoidedDate.HasValue ? "'" + coupon.VoidedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL",
                            coupon.Value,
                            coupon.SequenceId
                            );
                    }

                    /*
ValidationCode = 0
SingleUseCode = 1
SingleUseUrl = 2
OpenDate = 3
PlayedDate = 4
ClaimDate = 5
ClaimType = 6
ValidationDate = 7
ValidationValue = 8
VoidedDate = 9
Value = 10
SequenceId = 11*/

                }
            }
            catch (WebException ex)
            {
                
            }
        }

        public static string GenerateSalt()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SaltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public static string EncryptPassword(string password, string salt)
        {
            var sha = SHA512.Create();
            byte[] pwd512;
            string encryptedPassword = password;
            for (int i = 0; i < Pbkdf2Iterations; i++)
            {
                pwd512 = sha.ComputeHash(Encoding.UTF8.GetBytes(encryptedPassword + salt));
                encryptedPassword = Convert.ToBase64String(pwd512);
            }
            return encryptedPassword;
        }

        #region Zanox Test
        static void ZanoxTest(DateTime date)
        {
            string connectID = "D9D188F49CB8521B5157";
            string secrteKey = "6a741684831e4C+cacc8cf147aD893/30a9dc94A";
            string uri = "/reports/sales/date/" + date.ToString("yyyy-MM-dd");
            string method = "GET";
            string timestamp = DateTime.UtcNow.ToString("r");
            string nonce = CreateNounce(28);

            var stringToSign = method + uri + timestamp + nonce;

            var signature = HMACSHA1(secrteKey, stringToSign);
            string autorization = $"ZXWS {connectID}:{signature}";

            string apiroot = "https://api.zanox.com/json/2011-03-01" + uri;
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", autorization);
            request.Headers.Add("date", timestamp);
            request.Headers.Add("nonce", nonce);

            string ret = null;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    ret = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (ret == null)
                return;

            var jObj = JObject.Parse(ret);
            Console.WriteLine("Items: {0}", jObj["items"].ToString());
            if (jObj["items"].ToString() == "0")
                return;

            var list = jObj["saleItems"]["saleItem"].Children();

            using (StreamWriter w = File.AppendText(@"C:\ias\Projects\Rebens\CODE\API\Test\zanox.sql"))
            {
                foreach (var item in list)
                {
                    var sale = new ZanoxSale();
                    try
                    {
                        sale.ZanoxId = item["@id"].ToString();
                        sale.ReviewState = item["reviewState"].ToString();
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
                        sale.ClickId = long.Parse(item["clickId"].ToString());
                        sale.ClickInId = long.Parse(item["clickInId"].ToString());
                        sale.Amount = decimal.Parse(item["amount"].ToString());
                        sale.Commission = decimal.Parse(item["commission"].ToString());
                        sale.Currency = item["currency"].ToString();
                        sale.Created = sale.Modified = DateTime.Now;

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
                            if (item["gpps"]["gpp"] != null && item["gpps"]["gpp"] != null && item["gpps"]["gpp"]["@id"] != null)
                            {
                                if (item["gpps"]["gpp"]["@id"].ToString() == "zpar0")
                                    sale.Zpar = item["gpps"]["gpp"]["$"].ToString();

                                //    var gppItems = item["gpps"]["gpp"].Children();
                                //foreach (var gpp in gppItems)
                                //{
                                //    if (gpp["@id"].ToString() == "zpar0")
                                //        sale.Zpar = gpp["$"].ToString();
                                //}
                            }
                        }

                        if (string.IsNullOrEmpty(sale.Gpps) || string.IsNullOrEmpty(sale.Zpar))
                            continue;

                        sale.Status = (int)((ias.Rebens.Enums.ZanoxState)Enum.Parse(typeof(ias.Rebens.Enums.ZanoxState), sale.ReviewState));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try { 
                        w.WriteLine($"INSERT INTO ZanoxSale VALUES('{sale.ZanoxId}','{sale.ReviewState}','{(sale.TrackingDate.HasValue ? sale.TrackingDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL")}',"
                            + $"'{(sale.ModifiedDate.HasValue ? sale.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL")}','{(sale.ClickDate.HasValue ? sale.ClickDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL")}',"
                            + $"{sale.ClickId},{sale.ClickInId},{sale.Amount},{sale.Commission},'{sale.Currency}',{(sale.AdspaceId.HasValue ? sale.AdspaceId.Value.ToString() : "NULL")},"
                            + $"{(string.IsNullOrEmpty(sale.AdspaceValue) ? "NULL" : "'" + sale.AdspaceValue + "'")},{(sale.AdmediumId.HasValue ? sale.AdmediumId.Value.ToString() : "NULL")},"
                            + $"{(string.IsNullOrEmpty(sale.AdmediumValue) ? "NULL" : "'" + sale.AdmediumValue + "'")},{(sale.ProgramId.HasValue ? sale.ProgramId.Value.ToString() : "NULL")},"
                            + $"{(string.IsNullOrEmpty(sale.ProgramValue) ? "NULL" : "'" + sale.ProgramValue + "'")},{(string.IsNullOrEmpty(sale.ReviewNote) ? "NULL" : "'" + sale.ReviewNote + "'")},"
                            + $"'{sale.Gpps}','{sale.Zpar}',{sale.Status},GETDATE(),GETDATE(),1)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                    
            }
        }

        static string CreateNounce(int DefaultPasswordLength)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&^";
            string res = "";
            int length = DefaultPasswordLength;
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        static string HMACSHA1(string key, string dataToSign)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(key);
            HMACSHA1 hmac = new HMACSHA1(secretBytes);

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
            byte[] calcHash = hmac.ComputeHash(dataBytes);
            string calcHashString = Convert.ToBase64String(calcHash);
            return calcHashString;
        }
        #endregion Zanox Test
    }

    class Coupon
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public int IdCouponCampaign { get; set; }
        public string ValidationCode { get; set; }
        public string Campaign { get; set; }
        public string SingleUseCode { get; set; }
        public string SingleUseUrl { get; set; }
        public string WidgetValidationCode { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? PlayedDate { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string ClaimType { get; set; }
        public DateTime? ValidationDate { get; set; }
        public string ValidationValue { get; set; }
        public DateTime? VoidedDate { get; set; }
        public bool Locked { get; set; }
        public string Value { get; set; }
        public long SequenceId { get; set; }
        public int Status { get; set; }
        public DateTime VerifiedDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
