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
    //internal string ConnectionString = "Server=SURFACE\\SQLEXPRESS;Database=Rebens;user id=ias_user;password=k4r0l1n4;";
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Hours();

            Schedule<CouponToolsGenerateJob>().ToRunNow().AndEvery(1).Days().At(0, 30);
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);

            Schedule<WirecardJob>().ToRunNow().AndEvery(5).Minutes();

            Schedule<KeepAlive>().ToRunNow().AndEvery(15).Minutes();
        }
    }

    public class KeepAlive : IJob
    {
        public void Execute()
        {
            if (Constant.DebugOn)
            {
                var log = new LogErrorRepository(Constant.ConnectionString);
                log.Create("KeepAlive", "RUNNED", "", "");
            }
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
            var log = new LogErrorRepository(Constant.ConnectionString);
            var repo = new ZanoxSaleRepository(Constant.ConnectionString);

            log.Create("ZanoxUpdateJob", "START", "", "");
            int counter = 0;
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while (dt < DateTime.Now)
            {
                if (Constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), "");
                var list = zanox.UpdateZanoxSales(dt, out string error);
                if (Constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), $"total:{list.Count}");
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
            var log = new LogErrorRepository(Constant.ConnectionString);
            var customerRepo = new CustomerRepository(Constant.ConnectionString);
            var couponRepo = new CouponRepository(Constant.ConnectionString);

            log.Create("CouponToolsGenerateJob", "START", "", "");

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
                if (list != null && list.Count > 0)
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

            log.Create("CouponToolsGenerateJob", "FINISH", "", "");
            if (Constant.DebugOn)
            {
                var listDestinataries = new Dictionary<string, string>() { { "suporte@iasdigitalgroup.com", "Suporte" } };
                mail.Send(listDestinataries, "contato@rebens.com.br", "Rebens", "[Rebens] CouponToolsGenerateJob", "End at: " + DateTime.Now.ToString("HH:mm:ss"));
            }
        }
    }

    public class WirecardJob : IJob
    {
        public void Execute()
        {
            var log = new LogErrorRepository(Constant.ConnectionString);
            var notificationRepo = new MoipNotificationRepository(Constant.ConnectionString);
            log.Create("WirecardJob", "START", "", "");

            if (notificationRepo.HasSubscriptionToProcess())
            {
                notificationRepo.ProcessSubscription();
                log.Create("WirecardJob", "FINISH", "Subscriptions", "");
            }
            else if (notificationRepo.HasInvoicesToProcess())
            {
                notificationRepo.ProcessInvoices();
                log.Create("WirecardJob", "FINISH", "Invoices", "");
            }
            else if (notificationRepo.HasPaymentsToProcess())
            {
                notificationRepo.ProcessPayments();
                log.Create("WirecardJob", "FINISH", "Payments", "");
            }
        }
    }
}
