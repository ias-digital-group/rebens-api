using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ias.Rebens.api.helper
{
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
        }
    }
}
