using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

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
            var temp = HMACSHA1("israel@iasdigitalgroup.com", "israel@iasdigitalgroup.com|294.661.038-14");

            var encrypted = SimpleEncryption(temp + "||4||5");

            Console.WriteLine(encrypted);

            var decrypt = SimpleDecryption(encrypted);

            Console.WriteLine(decrypt);

            var arr = decrypt.Split("||");

            foreach( var s in arr)
                Console.WriteLine(s);

            //ZanoxTest();

            //int i = 0;
            //while (i < 100) 
            //{
            //    Console.WriteLine(HMACSHA1("israellow@outlook.com", "israellow@outlook.com|294.661.038-14"));
            //    i++;
            //}
            Console.ReadLine();
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
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    //if (!(response.Headers["Status"].StartsWith("200") || response.Headers["Status"].StartsWith("201") || response.Headers["Status"].StartsWith("202") || response.Headers["Status"].StartsWith("204")))
                    //    return false;

                    var ret = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Console.WriteLine("DONE: " + ret);

                    var jObj = JObject.Parse(ret);
                    var list = jObj["saleItems"]["saleItem"].Children();
                    

                    foreach (var item in list)
                        Console.WriteLine("id: {0} - amount: {1} - comission: {2} - zpar0: {3}", item["@id"].ToString(), item["amount"].ToString(), item["commission"].ToString(), item["gpps"]["gpp"]["$"].ToString());


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
            byte[] secretBytes = Encoding.UTF8.GetBytes(key);
            HMACSHA1 hmac = new HMACSHA1(secretBytes);

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
            byte[] calcHash = hmac.ComputeHash(dataBytes);
            string calcHashString = Convert.ToBase64String(calcHash);
            return calcHashString;
        }
        #endregion Zanox Test




    }
}
