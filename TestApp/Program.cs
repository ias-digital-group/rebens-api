using ias.Rebens;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

            //string connectionString = "Server=IAS-02;Database=Rebens;user id=ias_user;password=k4r0l1n4;";
            //int id = 3;



            sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
