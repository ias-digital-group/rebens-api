using ias.Rebens;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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

            var reader = new StreamReader(@"C:\ias\PROJECTS\Rebens\awin.csv");
            var writer = new StreamWriter(@"C:\ias\PROJECTS\Rebens\awin.txt");
            reader.ReadLine();
            var line = reader.ReadLine();
            int idx = 1;
            while (line != null)
            {
                var row = line.Split(';');
                if(row.Length == 39)
                {
                    if(row[30].Trim() != "")
                    {
                        //string urlDecoded = HttpUtility.UrlDecode(row[30]);
                        string urlDecoded = row[30].Replace("\"", "");
                        string zparDecoded;
                        try
                        {
                            zparDecoded = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(urlDecoded);
                            Console.WriteLine($"UPDATE ZanoxTemp SET [date] = '{row[4].Replace("\"", "")}' WHERE idBenefit = {zparDecoded.Split('|')[0]} AND idCustomer = {zparDecoded.Split('|')[1]}");
                            //    string tmp = $"insert into zanoxtemp ";
                            //    tmp += "(idbenefit,idcustomer,status,commision,total,userpercentage,usercommission,idzanoxsale,idx) values(";
                            //    tmp += $"{zparDecoded.Split('|')[0]}, {zparDecoded.Split('|')[1]}, '{row[5].Replace("\"", "")}',";
                            //    tmp += $"{row[3].Replace("\"", "").Replace(",", ".")},{row[2].Replace("\"", "").Replace(",", ".")}, null, null, null, {idx})";
                            //    writer.WriteLine(tmp);
                            //    idx++;
                        }
                        catch
                        {
                            zparDecoded = "error";
                        }
                        //Console.WriteLine($"zpar1: {row[30]} | {urlDecoded} | {zparDecoded}");
                        

                    }
                }

                line = reader.ReadLine();
            }

            writer.Close();
            reader.Close();
            writer.Dispose();
            reader.Dispose();

            //var zpar1 = HttpUtility.UrlDecode("ozxE75LALXbDmxwuQetQPw%3d%3d");
            //var zpar2 = HttpUtility.UrlDecode("hgB3N2OuVd3r96rDO8eZ8Q%3d%3d");
            //var zpar3 = HttpUtility.UrlDecode("hgB3N2OuVd3r96rDO8eZ8Q%3d%3d");

            //Console.WriteLine($"zpar1: {zpar1}");
            //Console.WriteLine($"zpar2: {zpar2}");
            //Console.WriteLine($"zpar3: {zpar3}");

            //var decode1 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption("NWAo1NrYx5T23jJO9bjhaw==");
            //var decode2 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(zpar2);
            //var decode3 = ias.Rebens.Helper.SecurityHelper.SimpleDecryption(zpar3);

            //Console.WriteLine($"decode1: {decode1}");
            //Console.WriteLine($"decode2: {decode2}");
            //Console.WriteLine($"decode3: {decode3}");

        
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
