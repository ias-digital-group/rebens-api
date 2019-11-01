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

            string connectionString = "Server=SURFACE\\SQLEXPRESS;Database=Rebens;user id=ias_user;password=k4r0l1n4;";


            using (var db = new RebensContext(connectionString))
            {
                var tmp = db.StaticText.Single(s => s.Id == 1845);

                var writer = new System.IO.StreamWriter("tmp.txt");
                writer.Write(tmp.Html);
                writer.Close();
            }




                sw.Stop();
            Console.WriteLine("Elapsed Time : " + sw.ElapsedMilliseconds + "ms");
        }

        public static ResultPage<Benefit> ListPage(int idOperation, int? idCategory, string benefitTypes, decimal? latitude, decimal? longitude, int page, int pageItems, string word, string sort, string idBenefits)
        {
            string connectionString = "Server=SURFACE\\SQLEXPRESS;Database=Rebens;user id=ias_user;password=k4r0l1n4;";
            object boundingBox = null;
            ResultPage<Benefit> ret;
            List<int> benefitIds = new List<int>();

            using (var db = new RebensContext(connectionString))
            {
                List<int> types = new List<int>();
                if (!string.IsNullOrEmpty(benefitTypes))
                {
                    var tmp = benefitTypes.Split(',');
                    foreach (var t in tmp)
                    {
                        if (int.TryParse(t, out int i))
                            types.Add(i);
                    }
                }


                List<int> listIds = new List<int>();
                if (!string.IsNullOrEmpty(idBenefits))
                {
                    foreach (var id in idBenefits.Split(',')) listIds.Add(int.Parse(id));
                }

                var tmpList = db.Benefit.Include("Partner")
                                .Where(b => !b.Deleted && !b.Partner.Deleted && ((!b.Exclusive && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)) || (b.Exclusive && b.IdOperation == idOperation))
                                    && (string.IsNullOrEmpty(word) || b.Title.Contains(word) || b.Name.Contains(word) || b.Call.Contains(word) || b.Partner.Name.Contains(word))
                                    && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                    && b.Active
                                    && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                    && (boundingBox == null || benefitIds.Any(bi => bi == b.Id))
                                    && !listIds.Any(i => i == b.Id)
                                );

                switch (sort.ToLower())
                {
                    case "title asc":
                        tmpList = tmpList.OrderBy(f => f.Title);
                        break;
                    case "title desc":
                        tmpList = tmpList.OrderByDescending(f => f.Title);
                        break;
                    case "id asc":
                        tmpList = tmpList.OrderBy(f => f.Id);
                        break;
                    case "id desc":
                        tmpList = tmpList.OrderByDescending(f => f.Id);
                        break;
                }

                var total = db.Benefit.Count(b => !b.Deleted && !b.Partner.Deleted && ((!b.Exclusive && b.BenefitOperations.Any(bo => bo.IdOperation == idOperation)) || (b.Exclusive && b.IdOperation == idOperation))
                                    && (string.IsNullOrEmpty(word) || b.Title.Contains(word) || b.Name.Contains(word) || b.Call.Contains(word) || b.Partner.Name.Contains(word))
                                    && (string.IsNullOrEmpty(benefitTypes) || types.Contains(b.IdBenefitType))
                                    && b.Active
                                    && (!idCategory.HasValue || (idCategory.HasValue && b.BenefitCategories.Any(bc => bc.IdCategory == idCategory.Value || bc.Category.IdParent == idCategory.Value)))
                                    && (boundingBox == null || benefitIds.Any(bi => bi == b.Id))
                                    );

                if (total < pageItems)
                    page = 0;

                int skip = page * pageItems;
                skip -= listIds.Count;

                var list = tmpList.Skip(skip).Take(pageItems).ToList();

                ret = new ResultPage<Benefit>(list, page, pageItems, total);
            }

            return ret;
        }
    }
}
