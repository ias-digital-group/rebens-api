using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace ias.Rebens.api.Controllers
{
    public class VoucherController : Controller
    {
        private IBenefitUseRepository benefitUseRepo;
        private IBenefitRepository benefitRepo;
        private ICustomerRepository customerRepo;
        private IOperationRepository operationRepo;
        private ICourseRepository courseRepo;
        private ICourseUseRepository courseUseRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="benefitUseRepository"></param>
        /// <param name="benefitRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="courseUseRepository"></param>
        /// <param name="courseRepository"></param>
        public VoucherController(IBenefitUseRepository benefitUseRepository, IBenefitRepository benefitRepository, ICustomerRepository customerRepository, 
            IOperationRepository operationRepository, ICourseUseRepository courseUseRepository, ICourseRepository courseRepository)
        {
            this.benefitUseRepo = benefitUseRepository;
            this.benefitRepo = benefitRepository;
            this.customerRepo = customerRepository;
            this.operationRepo = operationRepository;
            this.courseRepo = courseRepository;
            this.courseUseRepo = courseUseRepository;
        }


        /// <summary>
        /// Method that generate the voucher
        /// </summary>
        /// <param name="tp">Tipo do voucher ("b" = beneficio, "c" = curso)</param>
        /// <param name="code">Código enviado para o portal</param>
        /// <returns></returns>
        public IActionResult Index(string code, string tp)
        {
            Models.VoucherModel model = null;
            try
            {
                var ids = Helper.SecurityHelper.SimpleDecryption(code);
                var aIds = ids.Split('|');
                if (int.TryParse(aIds[0], out int id) && int.TryParse(aIds[1], out int idCustomer))
                {
                    var customer = customerRepo.Read(idCustomer, out string error);
                    var operation = operationRepo.Read(customer.IdOperation, out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        if (tp == "b")
                            model = GenerateBenefitVoucher(id, customer, operation, out error);
                        else if(tp == "c")
                            model = GenerateBenefitVoucher(id, customer, operation, out error);
                    }
                    
                    if(model != null)
                        return new ViewAsPdf("Index", "voucher.pdf", model);
                }
            }
            catch { }
            
            return View("Error"); 
        }

        private Models.VoucherModel GenerateBenefitVoucher(int idBenefit, Customer customer, Operation operation, out string error)
        {
            Models.VoucherModel model;
            var benefit = benefitRepo.Read(idBenefit, out error);

            var benefitUse = new BenefitUse()
            {
                Amount = benefit.MaxDiscountPercentage,
                Created = DateTime.Now,
                IdBenefit = benefit.Id,
                IdBenefitType = benefit.IdBenefitType,
                IdCustomer = customer.Id,
                Modified = DateTime.Now,
                Name = benefit.Name,
                Status = (int)Enums.BenefitUseStatus.NoCashBack
            };

            if (benefitUseRepo.Create(benefitUse, out error))
            {
                model = new Models.VoucherModel()
                {
                    ClubLogo = operation.Image,
                    Code = benefitUse.Code,
                    CustomerDoc = customer.Cpf,
                    CustomerName = customer.Name,
                    PartnerLogo = benefit.Partner.Logo,
                    Discount = benefit.MaxDiscountPercentage.Value.ToString().Substring(0, benefit.MaxDiscountPercentage.Value.ToString().IndexOf(".")) + "%",
                    ExpireDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                    HowToUse = benefit.VoucherText
                };
            }
            else
                model = null;

            return model;
        }

        private Models.VoucherModel GenerateCourseVoucher(int idCourse, Customer customer, Operation operation, out string error)
        {
            Models.VoucherModel model;
            var course = courseRepo.Read(idCourse, out error);

            var courseUse = new CourseUse()
            {
                Created = DateTime.Now,
                Discount = course.Discount,
                FinalPrice = course.FinalPrice,
                IdCourse = course.Id,
                IdCustomer = customer.Id,
                OriginalPrice = course.OriginalPrice,
                Modified = DateTime.Now,
                Name = course.Title,
                Status = (int)Enums.CourseUseStatus.Generated,
            };

            if (courseUseRepo.Create(courseUse, out error))
            {
                model = new Models.VoucherModel()
                {
                    ClubLogo = operation.Image,
                    Code = courseUse.Code,
                    CustomerDoc = customer.Cpf,
                    CustomerName = customer.Name,
                    PartnerLogo = course.College.Logo,
                    Discount = course.Discount.ToString().Substring(0, course.Discount.ToString().IndexOf(".")) + "%",
                    ExpireDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"),
                    HowToUse = course.VoucherText
                };
            }
            else
                model = null;

            return model;
        }
    }
}