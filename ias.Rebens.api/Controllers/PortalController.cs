using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Portal Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "customer, referal")]
    [ApiController]
    public class PortalController : ControllerBase
    {
        private IAddressRepository addrRepo;
        private IBannerRepository bannerRepo;
        private IBenefitRepository benefitRepo;
        private IBenefitUseRepository benefitUseRepo;
        private IBenefitViewRepository benefitViewRepo;
        private ICouponRepository couponRepo;
        private ICustomerRepository customerRepo;
        private ICustomerReferalRepository customerReferalRepo;
        private IFaqRepository faqRepo;
        private IFormContactRepository formContactRepo;
        private IFormEstablishmentRepository formEstablishmentRepo;
        private IMoipRepository moipRepo;
        private IOperationRepository operationRepo;
        private IOperationCustomerRepository operationCustomerRepo;
        private IStaticTextRepository staticTextRepo;
        private IWithdrawRepository withdrawRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bannerRepository"></param>
        /// <param name="benefitRepository"></param>
        /// <param name="faqRepository"></param>
        /// <param name="formContactRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="formEstablishmentRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="addressRepository"></param>
        /// <param name="withdrawRepository"></param>
        /// <param name="benefitUseRepository"></param>
        /// <param name="staticTextRepository"></param>
        /// <param name="couponRepository"></param>
        /// <param name="moipRepository"></param>
        /// <param name="customerReferalRepository"></param>
        /// <param name="operationCustomerRepository"></param>
        /// <param name="benefitViewRepository"></param>
        public PortalController(IBannerRepository bannerRepository, IBenefitRepository benefitRepository, IFaqRepository faqRepository, 
            IFormContactRepository formContactRepository, IOperationRepository operationRepository, IFormEstablishmentRepository formEstablishmentRepository, 
            ICustomerRepository customerRepository, IAddressRepository addressRepository, IWithdrawRepository withdrawRepository, 
            IBenefitUseRepository benefitUseRepository, IStaticTextRepository staticTextRepository, ICouponRepository couponRepository, 
            IMoipRepository moipRepository, ICustomerReferalRepository customerReferalRepository, IOperationCustomerRepository operationCustomerRepository,
            IBenefitViewRepository benefitViewRepository)
        {
            this.addrRepo = addressRepository;
            this.bannerRepo = bannerRepository;
            this.benefitRepo = benefitRepository;
            this.benefitUseRepo = benefitUseRepository;
            this.benefitViewRepo = benefitViewRepository;
            this.couponRepo = couponRepository;
            this.customerRepo = customerRepository;
            this.customerReferalRepo = customerReferalRepository;
            this.faqRepo = faqRepository;
            this.formContactRepo = formContactRepository;
            this.formEstablishmentRepo = formEstablishmentRepository;
            this.moipRepo = moipRepository;
            this.operationRepo = operationRepository;
            this.operationCustomerRepo = operationCustomerRepository;
            this.staticTextRepo = staticTextRepository;
            this.withdrawRepo = withdrawRepository;
        }

        /// <summary>
        /// Retorna os items da home não logada
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Retorna os items necessários para montar a home não logada</returns>
        /// <response code="200">Retorna o model com os items da home não logada, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("HomeLocked")]
        [ProducesResponseType(typeof(JsonDataModel<PortalHomeLockedModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetHomeLocked([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });


            var listFull = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Home, (int)Enums.BannerShow.HomeNotLogged, out string error);
            var listUnmissable = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Unmissable, (int)Enums.BannerShow.HomeNotLogged, out error);

            if (string.IsNullOrEmpty(error))
            {
                if ((listFull == null || listFull.Count == 0) && (listUnmissable == null || listUnmissable.Count == 0))
                    return NoContent();

                var ret = new JsonDataModel<PortalHomeLockedModel>()
                {
                    Data = new PortalHomeLockedModel()
                    {
                        BannerFullList = new List<PortalBannerModel>(),
                        BannerUnmissable = new List<PortalBannerModel>()
                    }
                };
                foreach (var banner in listUnmissable)
                    ret.Data.BannerUnmissable.Add(new PortalBannerModel(banner, null, null, null));

                foreach (var banner in listFull)
                {
                    string call = null, logo = null, title = null;
                    if (banner.IdBenefit.HasValue)
                        benefitRepo.ReadCallAndPartnerLogo(banner.IdBenefit.Value, out title, out call, out logo, out error);
                    ret.Data.BannerFullList.Add(new PortalBannerModel(banner, title, call, logo));
                }

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o texto da página requerida
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <param name="page">Página requerida</param>
        /// <returns>Retorna o objeto StaticText com as informações solicitadas</returns>
        /// <response code="200">Retorna o model, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("GetText")]
        [ProducesResponseType(typeof(JsonDataModel<StaticTextModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetText([FromQuery]string page, [FromHeader(Name = "x-operation-code")]string operationCode = null)
        {
            int idOperation = 0;
            Guid operationGuid = Guid.Empty;
            StaticText text = null;
            string error;
            if (string.IsNullOrEmpty(operationCode))
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

                    text = staticTextRepo.ReadText(idOperation, page, out error);
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
            }
            else
            {
                Guid.TryParse(operationCode, out operationGuid);
                if (operationGuid == Guid.Empty)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                text = staticTextRepo.ReadText(operationGuid, page, out error);
            }

            if (string.IsNullOrEmpty(error))
            {
                if (text == null || text.Id == 0)
                    return NoContent();

                var ret = new StaticTextModel(text);
                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salvar um contato
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="formContact"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("ContactForm")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ContactForm([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody] FormContactModel formContact)
        {
            var model = new JsonModel();
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var f = formContact.GetEntity();
                f.IdOperation = operation.Id;
                if (formContactRepo.Create(f, out error))
                {
                    var sendingBlue = new Integration.SendinBlueHelper();
                    var body = $"<p>Nome: {formContact.Name}<br />Email: {formContact.Email}<br />Telefone: {formContact.Phone}<br />Mensagem: {formContact.Message}</p>";
                    sendingBlue.Send("cluberebens@gmail.com", "Clube Rebens", "contato@rebens.com.br", "Contato", $"Novo Contato [{operation.Title}]", body);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Contato enviado com sucesso!", Id = f.Id });
                }
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salvar um formulário de indicação de benefícios
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="formEstablishment"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("EstablishmentForm")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult EstablishmentForm([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody] FormEstablishmentModel formEstablishment)
        {
            var model = new JsonModel();
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var f = formEstablishment.GetEntity();
                f.IdOperation = operation.Id;
                if (formEstablishmentRepo.Create(f, out error))
                {
                    var sendingBlue = new Integration.SendinBlueHelper();
                    string body = $"<p>Nome: {formEstablishment.Name}<br />Email: {formEstablishment.Email}<br />Estabelecimento: {formEstablishment.Establishment}<br />Site: {formEstablishment.WebSite}<br />Responsável: {formEstablishment.Responsible}<br />Email Responsável: {formEstablishment.ResponsibleEmail}<br />Cidade: {formEstablishment.City}<br />Estado: {formEstablishment.State}</p>";
                    sendingBlue.Send("cluberebens@gmail.com", "Clube Rebens", "contato@rebens.com.br", "Contato", $"Novo Contato [{operation.Title}]", body);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Indicação enviada com sucesso!", Id = f.Id });
                }
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Autentica um usuário na api
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="model"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns>O token e o usuário</returns>
        /// <respons code="200">se o usuário logar</respons>
        /// <respons code="400">se não encontrar o usuário ou a senha não estiver correta</respons>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(PortalTokenModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Login([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]LoginModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var customer = customerRepo.ReadByEmail(model.Email, operation.Id, out error);
                if (customer != null)
                {
                    if (customer.CheckPassword(model.Password))
                    {
                        var Data = LoadToken(customer, tokenConfigurations, signingConfigurations);
                        if(customer.CustomerType == (int)Enums.CustomerType.Customer)
                        {
                            decimal balance = (decimal)(new Random().NextDouble() * 499);
                            Data.balance = Math.Round(balance, 2);
                        }

                        return Ok(Data);
                    }
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "O login ou a senha não conferem!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Retorna os items da home logada
        /// </summary>
        /// <returns></returns>
        [HttpGet("Home")]
        [ProducesResponseType(typeof(JsonDataModel<PortalHomeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Home()
        {
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                if(!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
            }

            var listFull = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Home, (int)Enums.BannerShow.HomeLogged, out string error);
            var listUnmissable = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Unmissable, (int)Enums.BannerShow.HomeLogged, out error);
            var listBenefits = benefitRepo.ListForHomePortal(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if ((listFull == null || listFull.Count == 0) && (listUnmissable == null || listUnmissable.Count == 0))
                    return NoContent();

                var ret = new JsonDataModel<PortalHomeModel>()
                {
                    Data = new PortalHomeModel()
                    {
                        BannerFullList = new List<PortalBannerModel>(),
                        BannerUnmissable = new List<PortalBannerModel>(),
                        Benefits = new List<BenefitListItem>()
                    }
                };
                foreach (var banner in listUnmissable)
                    ret.Data.BannerUnmissable.Add(new PortalBannerModel(banner, null, null, null));

                foreach (var banner in listFull)
                {
                    string call = null, logo = null, title = null;
                    if (banner.IdBenefit.HasValue)
                        benefitRepo.ReadCallAndPartnerLogo(banner.IdBenefit.Value, out title, out call, out logo, out error);
                    ret.Data.BannerFullList.Add(new PortalBannerModel(banner, title, call, logo));
                }

                foreach (var item in listBenefits)
                    ret.Data.Benefits.Add(new BenefitListItem(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todos os benefícios da operação com paginação
        /// </summary>
        /// <param name="idCategory">categoria, não obrigatório (default=null)</param>
        /// <param name="idBenefitType">tipo de benefício, separado por vírgula, não obrigatório (default=null)</param>
        /// <param name="latitude">latitude do usuário (default=null)</param>
        /// <param name="longitude">longitude do usuário (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Benefits")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitListItem>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefits([FromQuery]int? idCategory = null, [FromQuery]string idBenefitType = null, [FromQuery]decimal? latitude = null, [FromQuery]decimal? longitude = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
            }

            ResultPage<Benefit> list;
            string error;
            if (page == 0 && !idCategory.HasValue && string.IsNullOrEmpty(idBenefitType) && !latitude.HasValue && !longitude.HasValue && string.IsNullOrEmpty(searchWord))
                list = benefitRepo.ListForHomeBenefitPortal(idOperation, out error);
            else
                list = benefitRepo.ListByOperation(idOperation, idCategory, idBenefitType, page, pageItems, searchWord, sort, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitListItem>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BenefitListItem>();
                foreach (var benefit in list.Page)
                    ret.Data.Add(new BenefitListItem(benefit));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o benefício conforme o ID
        /// </summary>
        /// <param name="id">Id do benefício</param>
        /// <returns>Parceiros</returns>
        /// <response code="200">Retorna o benefício, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Benefits/{id}")]
        [ProducesResponseType(typeof(JsonDataModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetBenefit(int id)
        {
            int idCustomer = 0;
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });

                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var operation = operationRepo.Read(idOperation, out string error);
            var benefit = benefitRepo.Read(id, out error);
            benefitViewRepo.SaveView(id, idCustomer, out string viewError);
            if (string.IsNullOrEmpty(error))
            {
                if (benefit == null || benefit.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<BenefitModel>() { Data = new BenefitModel(benefit, idCustomer) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("CustomerCreate")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CustomerCreate([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]CustomerModel customer)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var cust = customer.GetEntity();
                cust.IdOperation = operation.Id;

                if (customer.Address != null)
                {
                    var addr = customer.Address.GetEntity();
                    if (addrRepo.Create(addr, out error))
                        cust.IdAddress = addr.Id;
                }

                if (!string.IsNullOrEmpty(error))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });

                string password = Helper.SecurityHelper.CreatePassword();
                cust.SetPassword(password);
                if (customerRepo.Create(cust, out error))
                {
                    var sendingBlue = new Integration.SendinBlueHelper();
                    var body = $"<p>Seu cadastro foi realizado com sucesso!</p><p>Segue a sua senha temporária, sugerimos que você troque essa senha imediatamente:<br />Senha:<b>{password}</b></p>";
                    var result = sendingBlue.Send(cust.Email, cust.Name, "contato@rebens.com.br", operation.Title, "Cadatro realizado com sucesso", body);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = cust.Id });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="signUp"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SignUp([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]SignUpModel signUp)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                if(!customerRepo.CheckEmailAndCpf(signUp.Email, signUp.Cpf, operation.Id, out error))
                {
                    Customer customer = null;
                    CustomerReferal referal = null;
                    OperationCustomer oc = null;
                    if (operation.Id != 1)
                    {
                        oc = operationCustomerRepo.ReadByCpf(signUp.Cpf, out error);
                        if (oc != null)
                        {
                            customer = new Customer()
                            {
                                Email = signUp.Email,
                                Cpf = signUp.Cpf,
                                Name = oc.Name,
                                Phone = oc.Phone1,
                                Cellphone = oc.MobilePhone,
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                Status = (int)Enums.CustomerStatus.Validation,
                                CustomerType = (int)Enums.CustomerType.Customer,
                                Code = Helper.SecurityHelper.HMACSHA1(signUp.Email, signUp.Email + "|" + signUp.Cpf),
                                IdOperation = operation.Id
                            };
                        }
                        else
                        {
                            referal = customerReferalRepo.ReadByEmail(signUp.Email, out error);
                            if (referal != null)
                            {
                                customer = new Customer()
                                {
                                    Email = signUp.Email,
                                    Cpf = signUp.Cpf,
                                    Name = oc.Name,
                                    Created = DateTime.Now,
                                    Modified = DateTime.Now,
                                    Status = (int)Enums.CustomerStatus.Validation,
                                    CustomerType = (int)Enums.CustomerType.Referal,
                                    Code = Helper.SecurityHelper.HMACSHA1(signUp.Email, signUp.Email + "|" + signUp.Cpf),
                                    IdOperation = operation.Id
                                };
                            }
                            else
                                error = "O cpf digitado não está cadastrado em nossa base!";
                        }
                    }
                    else
                    {
                        customer = new Customer()
                        {
                            Email = signUp.Email,
                            Cpf = signUp.Cpf,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            Status = (int)Enums.CustomerStatus.Validation,
                            CustomerType = (int)Enums.CustomerType.Customer,
                            Code = Helper.SecurityHelper.HMACSHA1(signUp.Email, signUp.Email + "|" + signUp.Cpf),
                            IdOperation = operation.Id
                        };
                    }

                    if(customer != null)
                    {
                        if (customerRepo.Create(customer, out error))
                        {
                            Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, customer, out error);

                            if(operation.Id == 2)
                            {
                                if (oc != null)
                                    operationCustomerRepo.SetSigned(oc.Id, out error);
                                else if (referal != null)
                                    customerReferalRepo.ChangeStatus(referal.Id, Enums.CustomerReferalStatus.SignUp, out error);
                            }

                            return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Enviamos um e-mail para ativação do cadastro.", Id = customer.Id });
                        }
                    }
                    
                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Email ou CPF já cadastrado!" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="validateCustomer"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        /// <response code="200">Se o cliente for validado com sucesso já é retornado o token</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("ValidateCustomer")]
        [ProducesResponseType(typeof(PortalTokenModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ValidateCustomer([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]ValidateCustomerModel validateCustomer, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var customer = customerRepo.ReadByCode(validateCustomer.Code.Replace(" ", "+"), operation.Id, out error);

                if(customer != null)
                {
                    customer.SetPassword(validateCustomer.Password);
                    if(customerRepo.ChangePassword(customer.Id, customer.EncryptedPassword, customer.PasswordSalt, (int)Enums.CustomerStatus.Incomplete, out error))
                    {
                        var Data = LoadToken(customer, tokenConfigurations, signingConfigurations);
                        return Ok(Data);
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "changing password", Data = error });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "reading customer - " + validateCustomer.Code, Data = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida! (code: " + operationCode + ")" });
        }

        /// <summary>
        /// Atualiza um cliente
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut("CustomerUpdate")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CustomerUpdate([FromBody] CustomerModel customer, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var cust = customer.GetEntity();
            cust.IdOperation = idOperation;
            string error = null;

            if (customer.Address != null)
            {
                var addr = customer.Address.GetEntity();
                if (addr.Id > 0)
                {
                    if (addrRepo.Update(addr, out error))
                        cust.IdAddress = addr.Id;
                }
                else if (addrRepo.Create(addr, out error))
                    cust.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            cust.Status = (int)Enums.CustomerStatus.Active;
            if (customerRepo.Update(cust, out error))
            {
                var Data = LoadToken(cust, tokenConfigurations, signingConfigurations);
                if (idOperation == 1 && !string.IsNullOrEmpty(cust.Name))
                {
                    try
                    {
                        var coupon = new Coupon()
                        {
                            Campaign = "Raspadinha Unicap",
                            IdCustomer = cust.Id,
                            IdCouponCampaign = 1,
                            ValidationCode = Helper.SecurityHelper.GenerateCode(18),
                            Locked = false,
                            Status = (int)Enums.CouponStatus.pendent,
                            VerifiedDate = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            Modified = DateTime.UtcNow
                        };

                        var couponHelper = new Integration.CouponToolsHelper();
                        if (couponHelper.CreateSingle(cust, coupon, out error))
                            couponRepo.Create(coupon, out error);
                    }
                    catch { }
                }

                return Ok(Data);
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Altera a senha do cliente
        /// </summary>
        /// <param name="model">{ Id: id do cliente, OldPassword: senha antiga, NewPassword: nova senha, NewPasswordConfirm: confirmação da nova senha }</param>
        /// <returns></returns>
        /// <respons code="200"></respons>
        /// <respons code="400"></respons>
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var customer = customerRepo.Read(idCustomer, out string error);
            if (customer != null)
            {
                if (customer.CheckPassword(model.OldPassword))
                {
                    if (model.NewPassword == model.NewPasswordConfirm)
                    {
                        var salt = Helper.SecurityHelper.GenerateSalt();
                        var encryptedPassword = Helper.SecurityHelper.EncryptPassword(model.NewPassword, salt);
                        if (customerRepo.ChangePassword(idCustomer, encryptedPassword, salt, null, out error))
                            return Ok(new JsonModel() { Status = "ok" });

                        return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "A nova senha e a confirmação da nova senha devem ser iguais!" });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "A senha atual não confere!" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = string.IsNullOrEmpty(error) ? "Cliente não encontrado" : error });
        }

        /// <summary>
        /// Lembrete de senha
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="email">Email</param>
        /// <returns></returns>
        /// <respons code="200"></respons>
        /// <respons code="400"></respons>
        [AllowAnonymous]
        [HttpPost("RememberPassword")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RememberPassword([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]string email)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var user = customerRepo.ReadByEmail(email, operation.Id, out error);
                if (user != null)
                {
                    if(customerRepo.ChangeStatus(user.Id, Enums.CustomerStatus.ChangePassword, out error))
                    {
                        if(Helper.EmailHelper.SendPasswordRecovery(staticTextRepo, operation, user, out error))
                            return Ok(new JsonModel() { Status = "ok", Message = "Enviamos um link com as instruções para definir uma nova senha." });
                        return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Ocorreu um erro ao tentar enviar o lembrete da senha!" });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Ocorreu um erro ao tentar enviar o lembrete da senha!" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Retorna o cliente conforme o ID
        /// </summary>
        /// <returns>Parceiros</returns>
        /// <response code="200">Retorna o cliente, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("GetCustomer")]
        [ProducesResponseType(typeof(JsonDataModel<CustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetCustomer()
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var customer = customerRepo.Read(idCustomer, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (customer == null || customer.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CustomerModel>() { Data = new CustomerModel(customer) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna as informações necessárias para a página de Resgate
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna o resumo para página de resgate, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("GetWithdrawSummary"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonDataModel<BalanceSummaryModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetWithdrawSummary()
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            decimal blocked = Math.Round((decimal)(new Random().NextDouble() * 499), 2);
            decimal available = Math.Round((decimal)(new Random().NextDouble() * 499), 2);

            return Ok(new JsonDataModel<BalanceSummaryModel>() { Data = new BalanceSummaryModel() { AvailableBalance = available, BlokedBalance = blocked, Total = (available + blocked) } });
        }

        /// <summary>
        /// Retorna o histórico de Benefícios do usuário logado
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        /// <response code="200">Retorna o histórico de benefícios, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListBenefitHistory")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitUseModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitHistory([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var list = benefitUseRepo.ListByCustomer(idCustomer, page, pageItems, null, "date desc", out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitUseModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BenefitUseModel>();
                foreach (var benefitUse in list.Page)
                    ret.Data.Add(new BenefitUseModel(benefitUse));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Registra um resgate
        /// </summary>
        /// <param name="withdraw"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("Withdraw"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Withdraw([FromBody]WithdrawModel withdraw)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            if (idCustomer > 0)
            {

                var draw = new Withdraw()
                {
                    IdBankAccount = withdraw.IdBankAccount,
                    IdCustomer = idCustomer,
                    Amount = withdraw.Amount,
                    Date = DateTime.Now.Date,
                    Status = (int)Enums.WithdrawStatus.New,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                if (withdrawRepo.Create(draw, out string error))
                {

                    //var sendingBlue = new Integration.SendinBlueHelper();
                    //string body = $"<p>Nome: {formEstablishment.Name}<br />Email: {formEstablishment.Email}<br />Estabelecimento: {formEstablishment.Establishment}<br />Site: {formEstablishment.WebSite}<br />Responsável: {formEstablishment.Responsible}<br />Email Responsável: {formEstablishment.ResponsibleEmail}<br />Cidade: {formEstablishment.City}<br />Estado: {formEstablishment.State}</p>";
                    //sendingBlue.Send("cluberebens@gmail.com", "Clube Rebens", "contato@rebens.com.br", "Contato", $"[{operation.Title}] - Novo Resgate", body);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Resgate registrado com sucesso!", Id = draw.Id });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
        }

        /// <summary>
        /// Retorna o histórico de Resgates do cliente logado
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        /// <response code="200">Retorna o histórico de resgates do cliente, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListWithdraw"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(ResultPageModel<WithdrawItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListWithdraw([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var list = withdrawRepo.ListPage(idCustomer, page, pageItems, "date desc", out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<WithdrawItemModel>() {
                    Data = new List<WithdrawItemModel>(),
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,

                };
                foreach (var benefitUse in list.Page)
                    ret.Data.Add(new WithdrawItemModel(benefitUse));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o histórico de Cupons do cliente logado
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        /// <response code="200">Retorna o histórico de cupons do cliente, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Coupons")]
        [ProducesResponseType(typeof(ResultPageModel<CouponModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCoupons([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var list = couponRepo.ListPageByCustomer(idCustomer, page, pageItems, out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CouponModel>()
                {
                    Data = new List<CouponModel>(),
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,

                };
                foreach (var coupon in list.Page)
                    ret.Data.Add(new CouponModel(coupon));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a lista com os banners imperdíveis
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista com os banners Imperdíveis, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("UnmissableBanners")]
        [ProducesResponseType(typeof(ResultPageModel<BannerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListUnmissableBanners()
        {
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Unmissable, (int)Enums.BannerShow.Benefit, out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BannerModel>>()
                {
                    Data = new List<BannerModel>()
                };
                foreach (var banner in list)
                    ret.Data.Add(new BannerModel(banner));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna as perguntas e respostas da página faq
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna as perguntas e respostas da página faq, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Faq")]
        [ProducesResponseType(typeof(JsonDataModel<List<FaqModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Faq([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            if (operationGuid != null && operationGuid != Guid.Empty)
            {
                var list = faqRepo.ListByOperation(operationGuid, out string error);
                if (string.IsNullOrEmpty(error))
                {
                    if (list == null || list.Count == 0)
                        return NoContent();

                    var ret = new JsonDataModel<List<FaqModel>>() { Data = new List<FaqModel>() };
                    foreach (var faq in list)
                        ret.Data.Add(new FaqModel(faq));

                    return Ok(ret);
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Retorna uma lista com o histórico de pagamento do cliente
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns>Lista com o histórico de pagamento</returns>
        /// <response code="200">Retorna a lista com o histórico de pagamento, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListPayments"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonDataModel<List<PaymentModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPayments([FromQuery]int page = 0, int pageItems = 30)
        {
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
            }

            var list = moipRepo.ListPaymentsByCustomer(idCustomer, page, pageItems, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<PaymentModel>>()
                {
                    Data = new List<PaymentModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new PaymentModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        private PortalTokenModel LoadToken(Customer customer, helper.TokenOptions tokenConfigurations, helper.SigningConfigurations signingConfigurations)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                            new GenericIdentity(customer.Email),
                            new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, customer.Email)
                            }
                        );

            identity.AddClaim(new Claim(ClaimTypes.Role, customer.CustomerType == (int)Enums.CustomerType.Customer ? "customer" : "referal"));
            //foreach (var policy in user.Permissions)
            //    identity.AddClaim(new Claim("permissions", "permission1"));

            identity.AddClaim(new Claim("operationId", customer.IdOperation.ToString()));
            identity.AddClaim(new Claim("Id", customer.Id.ToString()));
            identity.AddClaim(new Claim("Name", string.IsNullOrEmpty(customer.Name) ? "" : customer.Name));
            identity.AddClaim(new Claim("Email", customer.Email));
            identity.AddClaim(new Claim("Status", ((Enums.CustomerStatus)customer.Status).ToString().ToLower()));

            DateTime dataCriacao = DateTime.UtcNow;
            DateTime dataExpiracao = dataCriacao.AddDays(2);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);


            var Data = new PortalTokenModel()
            {
                authenticated = true,
                created = dataCriacao,
                expiration = dataExpiracao,
                accessToken = token,
                balance = 0
            };

            return Data;
        }
    }
}