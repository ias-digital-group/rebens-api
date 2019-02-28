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
                    string howToUse = "";
                    if (benefit.StaticTexts != null)
                    {
                        var staticText = benefit.StaticTexts.SingleOrDefault(st => st.IdStaticTextType == (int)Enums.StaticTextType.BenefitHowToUse);
                        howToUse = staticText.Html;
                    }

                    var customer = customerRepo.Read(idCustomer, out error);

                    var benefitUse = new BenefitUse()
                    {
                        Amount = benefit.MaxDiscountPercentageOffline,
                        Created = DateTime.Now,
                        IdBenefit = benefit.Id,
                        IdBenefitType = benefit.IdBenefitType,
                        IdCustomer = customer.Id,
                        Modified = DateTime.Now,
                        Name = benefit.Title,
                        Status = (int)Enums.BenefitUseStatus.NoCashBack
                    };

                    var operation = operationRepo.Read(customer.IdOperation, out error);

                    if (benefitUseRepo.Create(benefitUse, out error))
                    {
                        model = new Models.VoucherModel()
                        {
                            ClubLogo = operation.Domain + "images/logo.jpg",
                            Code = benefitUse.Code,
                            CustomerDoc = customer.Cpf,
                            CustomerName = customer.Name,
                            PartnerLogo = benefit.Partner.Logo,
                            Discount = benefit.MaxDiscountPercentageOffline.Value.ToString().Substring(0, benefit.MaxDiscountPercentageOffline.Value.ToString().IndexOf(".")) + "%",
                            ExpireDate = benefit.DueDate.Value.ToString("dd/MM/yyyy"),
                            HowToUse = howToUse
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