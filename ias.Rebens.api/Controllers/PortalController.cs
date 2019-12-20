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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "customer")]
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
        private IBankAccountRepository bankAccountRepo;
        private IOperationPartnerRepository operationPartnerRepo;
        private ICourseRepository courseRepo;
        private ICourseCollegeRepository courseCollegeRepo;
        private ICourseGraduationTypeRepository courseGraduationTypeRepo;
        private ICoursePeriodRepository coursePeriodRepo;
        private ICourseModalityRepository courseModalityRepo;
        private ICourseViewRepository courseViewRepo;
        private IDrawRepository drawRepo;

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
        /// <param name="bankAccountRepository"></param>
        /// <param name="operationPartnerRepository"></param>
        /// <param name="courseRepository"></param>
        /// <param name="courseCollegeRepository"></param>
        /// <param name="courseGraduationTypeRepository"></param>
        /// <param name="courseModalityRepository"></param>
        /// <param name="coursePeriodRepository"></param>
        /// <param name="courseViewRepository"></param>
        /// <param name="drawRepository"></param>
        public PortalController(IBannerRepository bannerRepository, IBenefitRepository benefitRepository, IFaqRepository faqRepository, 
            IFormContactRepository formContactRepository, IOperationRepository operationRepository, IFormEstablishmentRepository formEstablishmentRepository, 
            ICustomerRepository customerRepository, IAddressRepository addressRepository, IWithdrawRepository withdrawRepository, 
            IBenefitUseRepository benefitUseRepository, IStaticTextRepository staticTextRepository, ICouponRepository couponRepository, 
            IMoipRepository moipRepository, ICustomerReferalRepository customerReferalRepository, IOperationCustomerRepository operationCustomerRepository,
            IBenefitViewRepository benefitViewRepository, IBankAccountRepository bankAccountRepository, IOperationPartnerRepository operationPartnerRepository,
            ICourseRepository courseRepository, ICourseCollegeRepository courseCollegeRepository, ICourseGraduationTypeRepository courseGraduationTypeRepository, 
            ICourseModalityRepository courseModalityRepository, ICoursePeriodRepository coursePeriodRepository, ICourseViewRepository courseViewRepository,
            IDrawRepository drawRepository)
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
            this.bankAccountRepo = bankAccountRepository;
            this.operationPartnerRepo = operationPartnerRepository;
            this.courseRepo = courseRepository;
            this.courseCollegeRepo = courseCollegeRepository;
            this.courseGraduationTypeRepo = courseGraduationTypeRepository;
            this.courseModalityRepo = courseModalityRepository;
            this.coursePeriodRepo = coursePeriodRepository;
            this.courseViewRepo = courseViewRepository;
            this.drawRepo = drawRepository;
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
            if(!Guid.TryParse(operationCode, out Guid operationGuid))
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);
            
            if (operation != null)
            {
                var mailTo = operationRepo.GetConfigurationOption(operation.Id, "form-email", out error);

                if (string.IsNullOrEmpty(mailTo)) mailTo = "cluberebens@gmail.com";

                var f = formContact.GetEntity();
                f.IdOperation = operation.Id;
                if (formContactRepo.Create(f, out error))
                {
                    var sendingBlue = new Integration.SendinBlueHelper();
                    var body = $"<p>Nome: {formContact.Name}<br />Email: {formContact.Email}<br />Telefone: {formContact.Phone}<br />Mensagem: {formContact.Message}</p>";
                    var listDestinataries = new Dictionary<string, string>() { { mailTo, operation.Title } };
                    sendingBlue.Send(listDestinataries, "contato@rebens.com.br", "Contato", $"Novo Contato [{operation.Title}]", body);

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
                    var listDestinataries = new Dictionary<string, string>() { { "cluberebens@gmail.com", "Clube Rebens" } };
                    sendingBlue.Send(listDestinataries, "contato@rebens.com.br", "Contato", $"Novo Contato [{operation.Title}]", body);

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
                if (customer != null && (customer.Status != (int)Enums.CustomerStatus.Inactive && customer.Status != (int)Enums.CustomerStatus.Validation))
                {
                    if (customer.CheckPassword(model.Password))
                    {
                        var Data = LoadToken(customer, tokenConfigurations, signingConfigurations);
                        return Ok(Data);
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "O login ou a senha não conferem!" });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "O usuário não encontrado!" });
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
        /// <param name="operationCode">categoria, não obrigatório (default=null)</param>
        /// <param name="idCategory">categoria, não obrigatório (default=null)</param>
        /// <param name="idBenefitType">tipo de benefício, separado por vírgula, não obrigatório (default=null)</param>
        /// <param name="latitude">latitude do usuário (default=null)</param>
        /// <param name="longitude">longitude do usuário (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="benefitIds">Lista dos benefícios que já foram exibidos</param>
        /// <param name="state">estado em que o benefício é válido</param>
        /// <param name="city">cidade em que o benefício é válido</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Benefits")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitListItem>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefits([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]int? idCategory = null, [FromQuery]string idBenefitType = null, 
                                [FromQuery]decimal? latitude = null, [FromQuery]decimal? longitude = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, 
                                [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]string benefitIds = "", [FromQuery]string state = "", [FromQuery]string city = "")
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);
            int idOperation = 0;
            string error;

            if (operationGuid == Guid.Empty)
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }

            if(idOperation == 0) return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            ResultPage<Benefit> list;
            if (idOperation != 1 && page == 0 && !idCategory.HasValue && string.IsNullOrEmpty(idBenefitType) && !latitude.HasValue && !longitude.HasValue && string.IsNullOrEmpty(searchWord))
                list = benefitRepo.ListForHomeBenefitPortal(idOperation, out error);
            else
                list = benefitRepo.ListByOperation(idOperation, idCategory, idBenefitType, latitude, longitude, page, pageItems, searchWord, sort, benefitIds, state, city, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0) return NoContent();

                var ret = new ResultPageModel<BenefitListItem>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BenefitListItem>()
                };

                foreach (var benefit in list.Page) ret.Data.Add(new BenefitListItem(benefit));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationCode">código da operação, não obrigatório (default=null)</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListBenefitStates")]
        [ProducesResponseType(typeof(JsonDataModel<List<FilterListItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitStates([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error;
            if (Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            else
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
                
            }

            if (idOperation == 0) return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = benefitRepo.ListStates(idOperation, out error);
            if (string.IsNullOrEmpty(error))
            {
                JsonDataModel<List<FilterListItem>> ret = new JsonDataModel<List<FilterListItem>>();
                ret.Data = list.Select(l => new FilterListItem()
                {
                    Value = l.Item1,
                    Text = l.Item2
                }).ToList();
                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationCode">código da operação, não obrigatório (default=null)</param>
        /// <param name="state">Estado, não obrigatório (default=null)</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListBenefitCities")]
        [ProducesResponseType(typeof(JsonDataModel<List<FilterListItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitCities([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]string state = null)
        {
            int idOperation = 0;
            string error;
            if (Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            else
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }

            }

            if (idOperation == 0) return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = benefitRepo.ListCities(idOperation, out error, state);
            if (string.IsNullOrEmpty(error))
            {
                JsonDataModel<List<FilterListItem>> ret = new JsonDataModel<List<FilterListItem>>();
                ret.Data = list.Select(l => new FilterListItem()
                            {
                                Value = l.Item1,
                                Text = l.Item2
                            }).ToList();
                
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
        [AllowAnonymous]
        [HttpGet("Benefits/{id}")]
        [ProducesResponseType(typeof(JsonDataModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetBenefit([FromHeader(Name = "x-operation-code")]string operationCode, int id)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);
            int idCustomer = 0;
            int idOperation = 0;
            string error;

            var principal = HttpContext.User;
            if (operationGuid == Guid.Empty)
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            if (idOperation == 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId != null)
                    int.TryParse(customerId.Value, out idCustomer);
            }

            var benefit = benefitRepo.Read(id, out error);
            if(idCustomer > 0)
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
                    var listDestinataries = new Dictionary<string, string>() { { cust.Email, cust.Name } };
                    var result = sendingBlue.Send(listDestinataries, "contato@rebens.com.br", operation.Title, "Cadatro realizado com sucesso", body);

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

            var operation = operationRepo.ReadForSignUp(operationGuid, out bool openSignUp, out string error);

            if (operation != null)
            {
                if(!customerRepo.CheckEmailAndCpf(signUp.Email, signUp.Cpf, operation.Id, out error))
                {
                    Customer customer = null;
                    CustomerReferal referal = null;
                    OperationCustomer oc = operationCustomerRepo.ReadByCpf(signUp.Cpf, operation.Id, out error);

                    if (oc != null)
                    {
                        customer = new Customer()
                        {
                            Email = signUp.Email,
                            Cpf = signUp.Cpf,
                            Name = oc.Name,
                            Phone = oc.Phone,
                            Cellphone = oc.Cellphone,
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
                        referal = customerReferalRepo.ReadByEmail(signUp.Email, operation.Id, out error);
                        if (referal != null)
                        {
                            customer = new Customer()
                            {
                                Email = signUp.Email,
                                Cpf = signUp.Cpf,
                                Name = referal.Name,
                                Created = DateTime.Now,
                                Modified = DateTime.Now,
                                Status = (int)Enums.CustomerStatus.Validation,
                                CustomerType = (int)Enums.CustomerType.Referal,
                                Code = Helper.SecurityHelper.HMACSHA1(signUp.Email, signUp.Email + "|" + signUp.Cpf),
                                IdOperation = operation.Id
                            };
                        }
                    }

                    if (customer == null && openSignUp)
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
                    else
                        error = "O cpf digitado não está cadastrado em nossa base!";

                    if(customer != null)
                    {
                        if (customerRepo.Create(customer, out error))
                        {
                            Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, customer, out error);

                            if (oc != null)
                                operationCustomerRepo.SetSigned(oc.Id, out error);
                            else if (referal != null)
                                customerReferalRepo.ChangeStatus(referal.Id, Enums.CustomerReferalStatus.SignUp, out error);

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
                Data.picture = string.IsNullOrEmpty(customer.Picture) ? "/images/default-avatar.png" : customer.Picture;
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

            if(benefitUseRepo.GetCustomerWithdrawSummary(idCustomer, out decimal available, out decimal blocked, out string error))
                return Ok(new JsonDataModel<BalanceSummaryModel>() { Data = new BalanceSummaryModel() { AvailableBalance = available, BlokedBalance = blocked, Total = (available + blocked) } });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
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
            int idOperation = 0;
            string customerName = "", customerEmail = "";
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
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });

                customerName = principal.Claims.SingleOrDefault(c => c.Type == "Name").Value;
                customerEmail = principal.Claims.SingleOrDefault(c => c.Type == "Email").Value;
            }

            if (idCustomer > 0)
            {
                if(withdraw.Amount < 25)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "O valor mínimo para resgate é de R$ 25,00." });

                decimal balance = benefitUseRepo.GetCustomerBalance(idCustomer, out string error);
                if(withdraw.Amount > balance)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = $"O valor máximo que você pode resgate é o seu saldo disponível." });

                var operation = operationRepo.Read(idOperation, out error);

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

                if (withdrawRepo.Create(draw, out error))
                {
                    var bankAccount = bankAccountRepo.Read(withdraw.IdBankAccount, out error);
                    var sendingBlue = new Integration.SendinBlueHelper();
                    string body = $"Novo resgate: <b>{customerName}</b><br /><br />Banco: {bankAccount.Bank.Name} - {bankAccount.Bank.Code}<br />";
                    body += $"Tipo de conta:{(bankAccount.Type == "CC" ? "Poupança" : "Conta Corrente")}<br />Agência:{bankAccount.Branch}<br />";
                    body += $"Número:{bankAccount.AccountNumber}<br />Valor:{withdraw.Amount.ToString("N")}";
                    var listDestinataries = new Dictionary<string, string>() { { "cluberebens@gmail.com", "Clube Rebens" } };
                    sendingBlue.Send(listDestinataries, "contato@rebens.com.br", "Contato", $"[{operation.Title}] - Novo Resgate", body);

                    string bodyCustomer = $"<p>Olá {customerName}, </p><br /><br /><p>Foi realizado um novo resgate conforme as informações abaixo:<br /><br />";
                    bodyCustomer += $"Banco: {bankAccount.Bank.Name} - {bankAccount.Bank.Code}<br />Tipo de conta:{(bankAccount.Type == "CC" ? "Poupança" : "Conta Corrente")}<br />Agência:{bankAccount.Branch}<br />";
                    bodyCustomer += $"Número:{bankAccount.AccountNumber}<br />Valor:{withdraw.Amount.ToString("N")}</p>";
                    Helper.EmailHelper.SendDefaultEmail(staticTextRepo, customerEmail, customerName, idOperation, $"[{operation.Title}] - Novo Resgate", bodyCustomer, out error);

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
        /// <param name="operationCode">código da operação, obrigatório</param>
        /// <returns></returns>
        /// <response code="200">Retorna a lista com os banners Imperdíveis, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("UnmissableBanners")]
        [ProducesResponseType(typeof(ResultPageModel<BannerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListUnmissableBanners([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);
            int idOperation = 0;
            string error;

            if (operationGuid == Guid.Empty)
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            if (idOperation == 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Unmissable, (int)Enums.BannerShow.Benefit, out error);
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
        /// <param name="operationCode">código da operação, obrigatório</param>
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
        [ProducesResponseType(typeof(ResultPageModel<PaymentModel>), 200)]
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
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new ResultPageModel<PaymentModel>()
                {
                    Data = new List<PaymentModel>(),
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,

                };

                foreach (var item in list)
                    ret.Data.Add(new PaymentModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista com os parceiros da operação
        /// </summary>
        /// <param name="operationCode">código da operação, obrigatório</param>
        /// <returns>Lista com os parceiros</returns>
        /// <response code="200">Retorna a lista com os parceiros, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListOperationPartners")]
        [ProducesResponseType(typeof(JsonDataModel<List<OperationPartnerModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListOperationPartners([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = operationPartnerRepo.ListActiveByOperation(operationGuid, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<OperationPartnerModel>>()
                {
                    Data = new List<OperationPartnerModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new OperationPartnerModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("SaveOperationPartnerCustomer")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveOperationPartnerCustomer([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]OperationPartnerCustomerModel model)
        {
            Guid.TryParse(operationCode, out Guid operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.ReadForSignUp(operationGuid, out bool openSignUp, out string error);
            if (operation != null)
            {
                var customer = model.GetEntity();
                customer.Status = (int)Enums.OperationPartnerCustomerStatus.newCustomer;
                if (operationPartnerRepo.CreateCustomer(customer, out error))
                {
                    string body = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {customer.Name}.</p><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Obrigado pelo cadastro!</p><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Enviamos sua solicitação para o RH validar seus dados e confirmar seu cadastro.</p>";
                    Helper.EmailHelper.SendDefaultEmail(staticTextRepo, customer.Email, customer.Name, operation.Id, $"{operation.Title} - Cadastro no site", body, out error);

                    body = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá,</p><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Recebemos a solicitação de cadastro para aprovação, acesse o sistema para avaliar a solicitação.</p><br /><p style=\"text-align:center;\"><a href=\"{Constant.URL}\" target=\"_blank\" style=\"display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#08061e;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;\">ACESSAR SISTEMA</a></p>";
                    var listDestinataries = operationPartnerRepo.ListDestinataries(customer.IdOperationPartner, out error);
                    Helper.EmailHelper.SendAdminEmail(listDestinataries, $"{operation.Title} - Novo cadastro de parceiro", body, out error);


                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Seu cadastro ira passar por um processo de validação e em breve entraremos em contato.", Id = customer.Id });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        #region Course
        /// <summary>
        /// Lista os cursos da operação com paginação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <param name="idCollege">faculdade, não obrigatório (default=null)</param>
        /// <param name="graduationTypes">tipos de graduação, array de inteiros, não obrigatório (default=null)</param>
        /// <param name="modalities">modalidades, array de inteiros, não obirgatório (default=null)</param>
        /// <param name="periods">períodos, array de inteiros, não obirgatório (default=null)</param>
        /// <param name="address">endereço, não obirgatório (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="state">Palavra à ser buscada</param>
        /// <param name="city">Palavra à ser buscada</param>
        /// <returns>Lista com os cursos encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Courses")]
        [ProducesResponseType(typeof(ResultPageModel<CourseItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCourses([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]int? idCollege = null, [FromQuery]string graduationTypes = null, 
            [FromQuery]string modalities = null, [FromQuery]string periods = null, [FromQuery]string address = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, 
            [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = null, [FromQuery]string courseBegin = null, [FromQuery]string state = "", [FromQuery]string city = "")
        {
            int idOperation = 0;
            string error = null;
            if (!Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, value: new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, value: new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            List<int> listModalities = null;
            List<int> listPeriods = null;
            List<int> listGraduationTypes = null;
            List<string> listCourseBegin = null;
            if(!string.IsNullOrEmpty(graduationTypes))
            {
                listGraduationTypes = new List<int>();
                var array = graduationTypes.Split(',');
                foreach(var id in array)
                    if (int.TryParse(id, out int idItem))
                        listGraduationTypes.Add(idItem);
            }
            if (!string.IsNullOrEmpty(periods))
            {
                listPeriods = new List<int>();
                var array = periods.Split(',');
                foreach (var id in array)
                    if (int.TryParse(id, out int idItem))
                        listPeriods.Add(idItem);
            }
            if (!string.IsNullOrEmpty(modalities))
            {
                listModalities = new List<int>();
                var array = modalities.Split(',');
                foreach (var id in array)
                    if (int.TryParse(id, out int idItem))
                        listModalities.Add(idItem);
            }
            if (!string.IsNullOrEmpty(courseBegin))
            {
                listCourseBegin = new List<string>();
                var array = courseBegin.Split(',');
                foreach (var item in array)
                    listCourseBegin.Add(item);
            }

            var list = courseRepo.ListForPortal(page: page, pageItems: pageItems, word: searchWord, sort: sort, idOperation: idOperation,
                idCollege: idCollege, graduationTypes: listGraduationTypes, modalities: listModalities, address: address, periods: listPeriods, 
                error: out error, courseBegin: listCourseBegin, state: state, city: city);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseItemModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CourseItemModel>()
                };
                foreach (var course in list.Page)
                    ret.Data.Add(new CourseItemModel(course));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o Curson conforme o ID
        /// </summary>
        /// <param name="id">Id do curso</param>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Curso</returns>
        /// <response code="200">Retorna o curso</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Courses/{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CourseItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetCourse(int id, [FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            int idCustomer = 0;
            string error = null;
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId != null)
                    int.TryParse(customerId.Value, out idCustomer);
            }

            if (operationGuid == Guid.Empty)
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var course = courseRepo.ReadForPortal(id, out error);
            if (string.IsNullOrEmpty(error))
            {
                if (course == null || course.Id == 0)
                    return NoContent();

                if(idCustomer > 0)
                    courseViewRepo.SaveView(id, idCustomer, out string viewError);
                return Ok(new JsonDataModel<CourseItemModel>() { Data = new CourseItemModel(course, idCustomer) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de graduação da operação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista com os tipos de graduação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("GraduationTypes")]
        [ProducesResponseType(typeof(ResultPageModel<CourseGraduationTypeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListGraduationTypes([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error = null;
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseGraduationTypeRepo.ListActive(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseGraduationTypeModel>()
                {
                    CurrentPage = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    ItemsPerPage = list.Count,
                    TotalItems = list.Count,
                    TotalPages = 1,
                    Data = new List<CourseGraduationTypeModel>()
                };
                foreach (var item in list)
                    ret.Data.Add(new CourseGraduationTypeModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as Modalidades da operação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista com as modalidades</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Modalities")]
        [ProducesResponseType(typeof(ResultPageModel<CourseModalityModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListModalities([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error = null;
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseModalityRepo.ListActive(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseModalityModel>()
                {
                    CurrentPage = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    ItemsPerPage = list.Count,
                    TotalItems = list.Count,
                    TotalPages = 1,
                    Data = new List<CourseModalityModel>()
                };
                foreach (var item in list)
                    ret.Data.Add(new CourseModalityModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os Períodos de inicio dos cursos da operação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista com as modalidades</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListCoursesBegin")]
        [ProducesResponseType(typeof(ResultPageModel<CourseModalityModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCoursesBegin([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error = null;
            if (!Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseRepo.ListCourseBegins(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new ResultPageModel<string>()
                {
                    CurrentPage = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    ItemsPerPage = list.Count,
                    TotalItems = list.Count,
                    TotalPages = 1,
                    Data = list
                };

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os Períodos da operação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista com os períodos</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Periods")]
        [ProducesResponseType(typeof(ResultPageModel<CoursePeriodModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPeriods([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error = null;
            
            if(!Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if (idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = coursePeriodRepo.ListActive(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new ResultPageModel<CoursePeriodModel>()
                {
                    CurrentPage = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    ItemsPerPage = list.Count,
                    TotalItems = list.Count,
                    TotalPages = 1,
                    Data = new List<CoursePeriodModel>()
                };
                foreach (var item in list)
                    ret.Data.Add(new CoursePeriodModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as Faculdades da operação
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista com as faculdades</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("Colleges")]
        [ProducesResponseType(typeof(ResultPageModel<CourseCollegeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListColleges([FromHeader(Name = "x-operation-code")]string operationCode = null)
        {
            int idOperation = 0;
            string error;
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
                idOperation = operationRepo.GetId(operationGuid, out error);

            if(idOperation <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseCollegeRepo.ListActive(idOperation, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseCollegeModel>()
                {
                    CurrentPage = 0,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    ItemsPerPage = list.Count,
                    TotalItems = list.Count,
                    TotalPages = 1,
                    Data = new List<CourseCollegeModel>()
                };
                foreach (var item in list)
                    ret.Data.Add(new CourseCollegeModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationCode">código da operação, não obrigatório (default=null)</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListCourseStates")]
        [ProducesResponseType(typeof(JsonDataModel<List<FilterListItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCourseStates([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            int idOperation = 0;
            string error;
            if (Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            else
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }

            }

            if (idOperation == 0) return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseRepo.ListStates(idOperation, out error);
            if (string.IsNullOrEmpty(error))
            {
                JsonDataModel<List<FilterListItem>> ret = new JsonDataModel<List<FilterListItem>>();
                ret.Data = list.Select(l => new FilterListItem()
                {
                    Value = l.Item1,
                    Text = l.Item2
                }).ToList();
                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationCode">código da operação, não obrigatório (default=null)</param>
        /// <param name="state">estado, não obrigatório (default=null)</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ListCourseCities")]
        [ProducesResponseType(typeof(JsonDataModel<List<FilterListItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCourseCities([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]string state = null)
        {
            int idOperation = 0;
            string error;
            if (Guid.TryParse(operationCode, out Guid operationGuid))
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            else
            {
                var principal = HttpContext.User;
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }

            }

            if (idOperation == 0) return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var list = courseRepo.ListCities(idOperation, out error, state);
            if (string.IsNullOrEmpty(error))
            {
                JsonDataModel<List<FilterListItem>> ret = new JsonDataModel<List<FilterListItem>>();
                ret.Data = list.Select(l => new FilterListItem()
                {
                    Value = l.Item1,
                    Text = l.Item2
                }).ToList();

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Course

        /// <summary>
        /// Retorna uma lista com os números da sorte
        /// </summary>
        /// <returns>Lista com os números</returns>
        /// <response code="200">Retorna a lista com os números, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListLuckNumbers"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonDataModel<List<DrawItemModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListLuckNumbers()
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

            var list = drawRepo.ListDrawItems(idCustomer, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<DrawItemModel>>()
                {
                    Data = new List<DrawItemModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new DrawItemModel(item));

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

            identity.AddClaim(new Claim(ClaimTypes.Role, "customer"));
            //foreach (var policy in user.Permissions)
            //    identity.AddClaim(new Claim("permissions", "permission1"));

            identity.AddClaim(new Claim("operationId", customer.IdOperation.ToString()));
            identity.AddClaim(new Claim("Id", customer.Id.ToString()));
            identity.AddClaim(new Claim("Name", string.IsNullOrEmpty(customer.Name) ? "" : customer.Name));
            identity.AddClaim(new Claim("Email", customer.Email));
            identity.AddClaim(new Claim("Status", ((Enums.CustomerStatus)customer.Status).ToString().ToLower()));
            identity.AddClaim(new Claim("cpf", customer.Cpf));

            if(customer.IdOperation == 1)
            {
                var plan = customerRepo.CheckPlanStatus(customer.Id);
                string planStatus = "-1";
                if(plan != null)
                {
                    switch(plan.Status.ToUpper())
                    {
                        case "ACTIVE": planStatus = "1";break;
                        case "CANCELED": planStatus = "2";break;
                        default: planStatus = "0";break;
                    }
                }
                identity.AddClaim(new Claim("planStatus", planStatus));
                identity.AddClaim(new Claim("subscriptionCode", plan != null ? plan.Code : ""));
            }

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
                accessToken = token
            };
            Data.balance = benefitUseRepo.GetCustomerBalance(customer.Id, out string error);
            Data.picture = string.IsNullOrEmpty(customer.Picture) ? "/images/default-avatar.png" : customer.Picture;

            return Data;
        }
    }
}