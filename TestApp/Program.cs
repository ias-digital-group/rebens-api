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
            int id = 3;

            using (var db = new RebensContext(connectionString))
            {
                var connection = ((SqlConnection)db.Database.GetDbConnection());
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "GenerateDrawItems";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@digits", 6);
                command.Parameters.AddWithValue("@totalItems", 400000);
                command.Parameters.AddWithValue("@counter", 300000);

                command.ExecuteNonQuery();
            }
            

            sw.Stop();

            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
