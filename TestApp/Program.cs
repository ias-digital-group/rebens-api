using ias.Rebens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();




            sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
