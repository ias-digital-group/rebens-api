using System;
using System.Net;
using System.Threading;
using FluentScheduler;

namespace ias.Rebens.api.helper
{
    public class SchedulerRegistry : Registry
    {
        public SchedulerRegistry()
        {
            Schedule<CouponToolsGenerateJob>().ToRunNow().AndEvery(1).Days().At(0, 30);
            //Schedule<CouponToolsUpdateJob>().ToRunEvery(1).Days().At(3, 0);
            //Schedule<ZanoxUpdateJob>().ToRunNow().AndEvery(2).Minutes();

            Schedule<KeepAlive>().ToRunEvery(15).Minutes();
        }
    }

    public class KeepAlive : IJob
    {
        public void Execute()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://admin.rebens.com.br/api/portal/homeLocked/");
                request.Method = "Get";
                request.ContentType = "application/xml";
                request.Headers.Add("x-operation-code", "de6e6ce9-1cf6-46b0-b5ea-2bdc09f359d1");
                var response = request.GetResponse();

                var mail = new Integration.SendinBlueHelper();
                mail.Send("suporte@iasdigitalgroup.com", "Suporte", "contato@rebens.com.br", "Rebens", "[Rebens] KeepAlive", "Run at: " + DateTime.Now.ToString("HH:mm:ss"));
            }
            catch { }
        }
    }

    public class ZanoxUpdateJob : IJob
    {
        IZanoxSaleRepository repo;
        public ZanoxUpdateJob(IZanoxSaleRepository saleRepository) { this.repo = saleRepository; }

        public void Execute()
        {
            var mail = new Integration.SendinBlueHelper();
            mail.Send("suporte@iasdigitalgroup.com", "Suporte", "contato@rebens.com.br", "Rebens", "[Rebens] ZanoxUpdateJob", "Start at: " + DateTime.Now.ToString("HH:mm:ss"));
            var zanox = new Integration.ZanoxHelper();
            var dt = DateTime.Now.AddMonths(-1);
            while(dt < DateTime.Now)
            {
                var list = zanox.UpdateZanoxSales(DateTime.Now, out string error);
                foreach(var item in list)
                {
                    repo.Save(item, out error);
                }

                dt = dt.AddDays(1);
            }
            
            mail.Send("suporte@iasdigitalgroup.com", "Suporte", "contato@rebens.com.br", "Rebens", "[Rebens] ZanoxUpdateJob", "End at: " + DateTime.Now.ToString("HH:mm:ss"));
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
        ICouponRepository couponRepo;
        ICustomerRepository customerRepo;
        public CouponToolsGenerateJob(ICouponRepository couponRepository, ICustomerRepository customerRepository)
        {
            this.couponRepo = couponRepository;
            this.customerRepo = customerRepository;
        }

        public void Execute()
        {
            var mail = new Integration.SendinBlueHelper();
            mail.Send("suporte@iasdigitalgroup.com", "Suporte", "contato@rebens.com.br", "Rebens", "[Rebens] CouponToolsGenerateJob", "Start at: " + DateTime.Now.ToString("HH:mm:ss"));
            while (true)
            {
                var couponHelper = new Integration.CouponToolsHelper();

                var list = customerRepo.ListToGenerateCoupon(1, 30);
                if (list != null && list.Count > 0)
                {
                    foreach (var customer in list)
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
                        Thread.Sleep(200);
                    }
                }

                if (customerRepo.HasToGenerateCoupon(1))
                    Thread.Sleep(800);
                else
                    break;
            }

            mail.Send("suporte@iasdigitalgroup.com", "Suporte", "contato@rebens.com.br", "Rebens", "[Rebens] CouponToolsGenerateJob", "End at: " + DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
