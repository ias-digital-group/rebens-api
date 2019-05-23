using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens.api.helper
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Hours();

            Schedule<CouponToolsGenerateJob>().ToRunNow().AndEvery(1).Days().At(0, 30);
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);

            Schedule<KeepAlive>().ToRunNow().AndEvery(15).Minutes();
        }
    }

    public class KeepAlive : IJob
    {
        public void Execute()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{Constant.URL}api/portal/homeLocked/");
                request.Method = "Get";
                request.ContentType = "application/xml";
                request.Headers.Add("x-operation-code", "de6e6ce9-1cf6-46b0-b5ea-2bdc09f359d1");
                var response = request.GetResponse();
            }
            catch { }
        }
    }

    public class ZanoxUpdateJob : IJob
    {
        public void Execute()
        {
            string conn = "Server=172.31.27.205;Database=Rebens;user id=Rebens_user;password=4KRe*d9!cd&g;";
            var log = new LogErrorRepository(conn);
            var repo = new ZanoxSaleRepository(conn);

            log.Create("ZanoxUpdateJob", "START", "", "");

            int counter = 0;
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while(dt < DateTime.Now)
            {
                log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), "");
                var list = zanox.UpdateZanoxSales(dt, out string error);
                log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), $"total:{list.Count}");
                counter += list.Count;
                foreach (var item in list)
                {
                    item.Zpar = string.IsNullOrEmpty(item.Zpar) ? "" : HttpUtility.UrlDecode(item.Zpar);
                    repo.Save(item, out error);
                }

                dt = dt.AddDays(1);
            }
            log.Create("ZanoxUpdateJob", "FINISH", "", "");
        }
    }

    public class CouponToolsUpdateJob : IJob
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }

    public class CouponToolsGenerateJob : IJob
    {
        public void Execute()
        {
            //string conn = "Server=IAS-02;Database=Rebens;user id=ias_user;password=k4r0l1n4;";
            //bool debug = false;
            string conn = "Server=172.31.27.205;Database=Rebens;user id=Rebens_user;password=i$f6LiF*N2kv;";
            bool debug = false;

            var customerRepo = new CustomerRepository(conn);
            var couponRepo = new CouponRepository(conn);

            var mail = new Integration.SendinBlueHelper();
            bool run = true;
            while (run)
            {
                var couponHelper = new Integration.CouponToolsHelper();
                List<Customer> list;
                try
                {
                    list = customerRepo.ListToGenerateCoupon(1, 30);
                }
                catch
                {
                    run = false;
                    break;
                }
                if(list != null && list.Count > 0)
                {
                    foreach (var customer in list)
                    {
                        try
                        {
                            var coupon = new Coupon()
                            {
                                Campaign = "Raspadinha Unicap",
                                IdCustomer = customer.Id,
                                IdCouponCampaign = 1,
                                ValidationCode = Helper.SecurityHelper.GenerateCode(18),
                                Locked = false,
                                Status = (int)Enums.CouponStatus.pendent,
                                VerifiedDate = DateTime.UtcNow,
                                Created = DateTime.UtcNow,
                                Modified = DateTime.UtcNow
                            };

                            if (couponHelper.CreateSingle(customer, coupon, out string error))
                            {
                                couponRepo.Create(coupon, out error);
                            }
                        }
                        catch
                        {
                        }
                        Thread.Sleep(200);
                    }
                }


                if (customerRepo.HasToGenerateCoupon(1))
                    Thread.Sleep(800);
                else
                {
                    run = false;
                    break;
                }
            }
            if (debug)
            {
                var listDestinataries = new Dictionary<string, string>() { { "suporte@iasdigitalgroup.com", "Suporte" } };
                mail.Send(listDestinataries, "contato@rebens.com.br", "Rebens", "[Rebens] CouponToolsGenerateJob", "End at: " + DateTime.Now.ToString("HH:mm:ss"));
            }
        }
    }
}
