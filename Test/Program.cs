using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new Random();

            string[] benefits = { "Asus", "Avon Store", "Natura Center", "Yázigi", "YouCom", "Submarino", "Americanas", "Box do Churrasco", "Avon", "Colombo" };
            string[] status = { "Cashback em Processamento", "Cashback Disponível", "Não Possuí Cashback", "Reemetir" };

            int itemsCount = rnd.Next(0, 11);

            for (int i = 0; i < itemsCount; i++)
            {
                var benefitIdx = rnd.Next(0, 9);
                var amount = (decimal)(new Random().NextDouble() * 499);
                var st = status[rnd.Next(0, 4)];
                decimal cashback = 0;
                if (st == "Cashback em Processamento" || st == "Cashback Disponível")
                    cashback = (amount * (decimal)(rnd.NextDouble() * .1));
                Console.WriteLine("{0} | {1}-{2} | {3} | {4} | {5} | {6} | {7} | {8}", rnd.Next(), (benefitIdx + 1), benefits[benefitIdx], 
                    DateTime.Now.AddDays(-rnd.Next(0,60)).ToString("dd/MM/yyyy"), amount, cashback, st, Math.Round(amount, 2), Math.Round(cashback, 2));
            }

            Console.ReadLine();
        }

        #region Zanox Test
        static void ZanoxTest()
        {
            string connectID = "D9D188F49CB8521B5157";
            string secrteKey = "6a741684831e4C+cacc8cf147aD893/30a9dc94A";
            string uri = "/reports/sales/date/2019-02-08";
            string method = "GET";
            string timestamp = DateTime.UtcNow.ToString("r");
            string nonce = CreateNounce(28);

            Console.WriteLine("timestamp: {0}", timestamp);
            Console.WriteLine("nonce: {0}", nonce);

            var stringToSign = method + uri + timestamp + nonce;
            Console.WriteLine("stringToSign: {0}", stringToSign);

            var signature = HMACSHA1(secrteKey, stringToSign);
            Console.WriteLine("signature: {0}", signature);
            string autorization = $"ZXWS {connectID}:{signature}";
            Console.WriteLine("Authorization: {0}", autorization);

            string apiroot = "https://api.zanox.com/json/2011-03-01" + uri;
            var request = (HttpWebRequest)WebRequest.Create(apiroot);
            request.Method = method;
            request.Timeout = 50000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", autorization);
            request.Headers.Add("date", timestamp);
            request.Headers.Add("nonce", nonce);

            //if (serializedObject != null)
            //{
            //    request.AllowWriteStreamBuffering = false;
            //    request.SendChunked = false;

            //    UTF8Encoding encoding = new UTF8Encoding();
            //    byte[] bytes = encoding.GetBytes(serializedObject);
            //    request.ContentLength = bytes.Length;
            //    using (Stream writeStream = request.GetRequestStream())
            //    {
            //        writeStream.Write(bytes, 0, bytes.Length);
            //    }
            //}

            try
            {
                using (var response = request.GetResponse())
                {
                    //if (!(response.Headers["Status"].StartsWith("200") || response.Headers["Status"].StartsWith("201") || response.Headers["Status"].StartsWith("202") || response.Headers["Status"].StartsWith("204")))
                    //    return false;

                    var ret = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Console.WriteLine(ret);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            Byte[] secretBytes = UTF8Encoding.UTF8.GetBytes(key);
            HMACSHA1 hmac = new HMACSHA1(secretBytes);

            Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(dataToSign);
            Byte[] calcHash = hmac.ComputeHash(dataBytes);
            String calcHashString = Convert.ToBase64String(calcHash);
            return calcHashString;
        }
        #endregion Zanox Test




    }
}
