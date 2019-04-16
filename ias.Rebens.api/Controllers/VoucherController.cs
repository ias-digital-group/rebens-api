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

        public VoucherController(IBenefitUseRepository benefitUseRepository, IBenefitRepository benefitRepository, ICustomerRepository customerRepository, IOperationRepository operationRepository)
        {
            this.benefitUseRepo = benefitUseRepository;
            this.benefitRepo = benefitRepository;
            this.customerRepo = customerRepository;
            this.operationRepo = operationRepository;
        }

        public IActionResult Index(string code)
        {
            Models.VoucherModel model;
            try
            {
                var ids = Helper.SecurityHelper.SimpleDecryption(code);
                var aIds = ids.Split('|');
                if (int.TryParse(aIds[0], out int idBenefit) && int.TryParse(aIds[1], out int idCustomer))
                {
                    var benefit = benefitRepo.Read(idBenefit, out string error);
                    var customer = customerRepo.Read(idCustomer, out error);

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

                    var operation = operationRepo.Read(customer.IdOperation, out error);

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

                        return new ViewAsPdf("Index", "voucher.pdf", model);
                    }
                }
            }
            catch { }
            
            return View("Error"); 
        }
    }
}