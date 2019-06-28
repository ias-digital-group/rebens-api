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

            string connectionString = "Server=IAS-02;Database=Rebens;user id=ias_user;password=k4r0l1n4;";
            //int id = 3;

            using (var db = new RebensContext(connectionString))
            {
                var dt = DateTime.Now.Date;
                var listDraws = db.Draw.Where(d => d.Active && !d.Deleted && d.Generated && d.Operation.Active && d.StartDate >= dt && d.EndDate <= dt);
                if (listDraws != null && listDraws.Count() > 0)
                {
                    foreach (var draw in listDraws)
                    {
                        var listCustomers = db.Customer.Where(c => c.IdOperation == draw.IdOperation);
                        foreach (var customer in listCustomers)
                        {
                            if (!db.DrawItem.Any(d => d.IdCustomer == customer.Id && d.IdDraw == draw.Id && d.Modified.Year == DateTime.Now.Year && d.Modified.Month == DateTime.Now.Month))
                            {
                                var item = db.DrawItem.Where(d => d.IdDraw == draw.Id && !d.IdCustomer.HasValue).OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                                if (item != null)
                                {
                                    item.IdCustomer = customer.Id;
                                    item.Modified = DateTime.Now;
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
