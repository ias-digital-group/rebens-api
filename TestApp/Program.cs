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

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var ret = TestAWS();

            Task.WaitAll(ret);

            Console.WriteLine($"result: {ret}");

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
