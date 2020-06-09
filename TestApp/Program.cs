using ias.Rebens;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var zpar1 = HttpUtility.UrlDecode("ozxE75LALXbDmxwuQetQPw%3d%3d");
            var zpar2 = HttpUtility.UrlDecode("hgB3N2OuVd3r96rDO8eZ8Q%3d%3d");
            var zpar3 = HttpUtility.UrlDecode("hgB3N2OuVd3r96rDO8eZ8Q%3d%3d");

            Console.WriteLine($"zpar1: {zpar1}");
            Console.WriteLine($"zpar2: {zpar2}");
            Console.WriteLine($"zpar3: {zpar3}");

            var decode1 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(zpar1);
            var decode2 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(zpar2);
            var decode3 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(zpar3);

            Console.WriteLine($"decode1: {decode1}");
            Console.WriteLine($"decode2: {decode2}");
            Console.WriteLine($"decode3: {decode3}");

        
            //Task.WaitAll(ret);

            //Console.WriteLine($"result: {ret}");

            sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }

        public async static Task<bool> TestAWS()
        {
            var aws = new ias.Rebens.Integration.AWSHelper();
            return await aws.DisableBucketAsync("ias.sistemarebens.com.br");
        }
    }
}
