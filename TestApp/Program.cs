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


            //var dt = new DateTime(2020, 3, 1);
            //for(int i = 1; i<29; i++)
            //{
            //    Console.WriteLine($"Qty: {i}");
            //    var daysInBetween28 = Convert.ToInt32(28/i);
            //    var daysInBetween29 = Convert.ToInt32(29/i);
            //    var daysInBetween30 = Convert.ToInt32(30/i);
            //    var daysInBetween31 = Convert.ToInt32(31/i);

            //    var tmpDt = dt;
            //    for(int j = 0; j < i; j++)
            //    {
            //        Console.WriteLine($"28 - {dt.AddDays(j * daysInBetween28).Date}");
            //        Console.WriteLine($"29 - {dt.AddDays(j * daysInBetween29).Date}");
            //        Console.WriteLine($"30 - {dt.AddDays(j * daysInBetween30).Date}");
            //        Console.WriteLine($"31 - {dt.AddDays(j * daysInBetween31).Date}");

            //    }
            //}

            Console.WriteLine(TimeZoneInfo.Local.DisplayName);

            



            sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
