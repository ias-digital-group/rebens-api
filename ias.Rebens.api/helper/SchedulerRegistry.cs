using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using FluentScheduler;

namespace ias.Rebens.api.helper
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            //Schedule<GenerateDrawItemsJob>().ToRunNow().AndEvery(15).Minutes();
            //Schedule<DistributeNumbersJob>().ToRunNow().AndEvery(1).Months();
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);
            //Schedule<CouponToolsGenerateJob>().ToRunNow().AndEvery(1).Days().At(0, 30);

            Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Hours();
            Schedule<WirecardJob>().ToRunNow().AndEvery(15).Minutes();
            Schedule<KeepAlive>().ToRunNow().AndEvery(15).Minutes();
        }
    }

    /// <summary>
    /// Keep Alive
    /// </summary>
    public class KeepAlive : IJob
    {
        private Constant constant;
        public KeepAlive()
        {
            this.constant = new Constant();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            if (constant.DebugOn)
            {
                var log = new LogErrorRepository(constant.ConnectionString);
                log.Create("KeepAlive", "RUNNED", "", "");
            }
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri($"{constant.URL}api/portal/homeLocked/"));
                request.Method = "Get";
                request.ContentType = "application/xml";
                request.Headers.Add("x-operation-code", "de6e6ce9-1cf6-46b0-b5ea-2bdc09f359d1");
                var response = request.GetResponse();
            }
            catch { }
        }
    }

    /// <summary>
    /// Zanox Update Job
    /// </summary>
    public class ZanoxUpdateJob : IJob
    {
        private Constant constant;
        public ZanoxUpdateJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var log = new LogErrorRepository(constant.ConnectionString);
            var repo = new ZanoxSaleRepository(constant.ConnectionString);

            log.Create("ZanoxUpdateJob", "START", "", "");
            int counter = 0;
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while (dt < DateTime.Now)
            {
                if (constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), "");
                var list = zanox.UpdateZanoxSales(dt, out string error);
                if (constant.DebugOn) log.Create("ZanoxUpdateJob", "GetByDate", dt.ToString("dd/MM/yyyy"), $"total:{list.Count}");
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

    /// <summary>
    /// Coupon Tools Update Job
    /// </summary>
    public class CouponToolsUpdateJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Coupon Tools Generate Job
    /// </summary>
    public class CouponToolsGenerateJob : IJob
    {
        private Constant constant;
        public CouponToolsGenerateJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var log = new LogErrorRepository(constant.ConnectionString);
            var customerRepo = new CustomerRepository(constant.ConnectionString);
            var couponRepo = new CouponRepository(constant.ConnectionString);

            log.Create("CouponToolsGenerateJob", "START", "", "");

            //var mail = new Integration.SendinBlueHelper();
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
            //if (Constant.DebugOn)
            //{
            //    var listDestinataries = new Dictionary<string, string>() { { "suporte@iasdigitalgroup.com", "Suporte" } };
            //    mail.Send(listDestinataries, "contato@rebens.com.br", "Rebens", "[Rebens] CouponToolsGenerateJob", "End at: " + DateTime.Now.ToString("HH:mm:ss"));
            //}
        }
    }

    /// <summary>
    /// Wirecard Job
    /// </summary>
    public class WirecardJob : IJob
    {
        private Constant constant;
        public WirecardJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            var log = new LogErrorRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
            var wireRepo = new WirecardPaymentRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
            var orderRepo = new OrderRepository(constant.AppSettings.ConnectionStrings.DefaultConnection);
            log.Create("WirecardJob", "START", "", "");

            if (wireRepo.HasPaymentToProcess())
            {
                log.Create("WirecardJob.Payments", "START", "", "");
                wireRepo.ProcessPayments();
                log.Create("WirecardJob.Payments", "FINISH", "", "");
            }
            else
                log.Create("WirecardJob.Payments", "NO-PAYMENTS", "", "");


            if (orderRepo.HasOrderToProcess())
            {
                log.Create("WirecardJob.Orders", "START", "", "");
                orderRepo.ProcessOrder();
                log.Create("WirecardJob.Orders", "FINISH", "", "");
            }
            else
                log.Create("WirecardJob.Orders", "NO-ORDERS", "", "");
        }
    }

    /// <summary>
    /// Generate Draw Items
    /// </summary>
    public class GenerateDrawItemsJob : IJob
    {
        private Constant constant;
        public GenerateDrawItemsJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            try
            {
                if (constant.DebugOn)
                {
                    var log = new LogErrorRepository(constant.ConnectionString);
                    log.Create("GenerateDrawItemsJob", "RUNNED", "", "");
                }
                var repo = new DrawRepository(constant.ConnectionString);
                repo.GenerateItems();
            }
            catch { }
        }
    }

    /// <summary>
    /// Generate Numbers Job
    /// </summary>
    public class DistributeNumbersJob : IJob
    {
        private Constant constant;
        public DistributeNumbersJob()
        {
            this.constant = new Constant();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            try
            {
                var log = new LogErrorRepository(constant.ConnectionString);
                log.Create("DistributeNumbersJob", "START", "", "");
                var repo = new DrawRepository(constant.ConnectionString);
                repo.DistributeNumbers();
                log.Create("DistributeNumbersJob", "END", "", "");
            }
            catch { }
        }
    }
}
