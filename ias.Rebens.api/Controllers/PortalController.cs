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
        private IFaqRepository faqRepo;
        private IFormContactRepository formContactRepo;
        private IFormEstablishmentRepository formEstablishmentRepo;
        private IOperationRepository operationRepo;
        private ICustomerRepository customerRepo;
        private IWithdrawRepository withdrawRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addressRepository"></param>
        /// <param name="bannerRepository"></param>
        /// <param name="benefitRepository"></param>
        /// <param name="benefitUseRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="faqRepository"></param>
        /// <param name="formContactRepository"></param>
        /// <param name="formEstablishmentRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="withdrawRepository"></param>
        public PortalController(IBannerRepository bannerRepository, IBenefitRepository benefitRepository, IFaqRepository faqRepository, 
            IFormContactRepository formContactRepository, IOperationRepository operationRepository, IFormEstablishmentRepository formEstablishmentRepository, 
            ICustomerRepository customerRepository, IAddressRepository addressRepository, IWithdrawRepository withdrawRepository, 
            IBenefitUseRepository benefitUseRepository)
        {
            this.bannerRepo = bannerRepository;
            this.benefitRepo = benefitRepository;
            this.faqRepo = faqRepository;
            this.formContactRepo = formContactRepository;
            this.operationRepo = operationRepository;
            this.formEstablishmentRepo = formEstablishmentRepository;
            this.customerRepo = customerRepository;
            this.addrRepo = addressRepository;
            this.withdrawRepo = withdrawRepository;
            this.benefitUseRepo = benefitUseRepository;
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


            var listFull = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Home, out string error);
            var listUnmissable = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Unmissable, out error);

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
                    ret.Data.BannerUnmissable.Add(new PortalBannerModel(banner, null, null));

                foreach (var banner in listFull)
                {
                    string call = null, logo = null;
                    if (banner.IdBenefit.HasValue)
                        benefitRepo.ReadCallAndPartnerLogo(banner.IdBenefit.Value, out call, out logo, out error);
                    ret.Data.BannerFullList.Add(new PortalBannerModel(banner, call, logo));
                }

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista com as perguntas frequentes
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista das perguntas frequentes</returns>
        /// <response code="200">Retorna a lista das perguntas frequentes, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListFaq")]
        [ProducesResponseType(typeof(JsonDataModel<List<FaqModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListFaq([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });


            var list = faqRepo.ListByOperation(operationGuid, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<FaqModel>>()
                {
                    Data = new List<FaqModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new FaqModel(item));

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
                    //var sendingBlue = new Integration.SendinBlueHelper();
                    //sendingBlue.Send(email.ToEmail, email.ToName, email.FromEmail, email.FromName, email.Subject, email.Message);

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
                    //var sendingBlue = new Integration.SendinBlueHelper();
                    //sendingBlue.Send(email.ToEmail, email.ToName, email.FromEmail, email.FromName, email.Subject, email.Message);

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
                        ClaimsIdentity identity = new ClaimsIdentity(
                            new GenericIdentity(model.Email),
                            new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, model.Email)
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

                        decimal balance = (decimal)(new Random().NextDouble() * 499);

                        var Data = new PortalTokenModel()
                        {
                            authenticated = true,
                            created = dataCriacao,
                            expiration = dataExpiracao,
                            accessToken = token,
                            balance = Math.Round(balance, 2)
                        };

                        return Ok(Data);
                    }
                }
                return NotFound(new JsonModel() { Status = "error", Message = "O login ou a senha não conferem!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
            // TO READ CLAIMS
            /*
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"CLAIM TYPE: {claim.Type}; CLAIM VALUE: {claim.Value}");
                }
            }
            */
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

            var listFull = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Home, out string error);
            var listUnmissable = bannerRepo.ListByTypeAndOperation(idOperation, (int)Enums.BannerType.Unmissable, out error);
            var listBenefits = benefitRepo.ListByOperation(idOperation, null, null, 0, 6, null, "title asc", out error);

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
                        Benefits = new List<PortalBenefitModel>()
                    }
                };
                foreach (var banner in listUnmissable)
                    ret.Data.BannerUnmissable.Add(new PortalBannerModel(banner, null, null));

                foreach (var banner in listFull)
                {
                    string call = null, logo = null;
                    if (banner.IdBenefit.HasValue)
                        benefitRepo.ReadCallAndPartnerLogo(banner.IdBenefit.Value, out call, out logo, out error);
                    ret.Data.BannerFullList.Add(new PortalBannerModel(banner, call, logo));
                }

                foreach (var item in listBenefits)
                    ret.Data.Benefits.Add(new PortalBenefitModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todos os benefícios da operação com paginação
        /// </summary>
        /// <param name="idCategory">categoria, não obrigatório (default=null)</param>
        /// <param name="benefitTypes">tipo de benefício, separado por vírgula, não obrigatório (default=null)</param>
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
        public IActionResult ListBenefits([FromQuery]int? idCategory = null, [FromQuery]string benefitTypes = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
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

            var list = benefitRepo.ListByOperation(idOperation, idCategory, benefitTypes, page, pageItems, searchWord, sort, out string error);

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
        public IActionResult Get(int id)
        {
            var benefit = benefitRepo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (benefit == null || benefit.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<BenefitModel>() { Data = new BenefitModel(benefit) });
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
                    var customer = new Customer()
                    {
                        Email = signUp.Email,
                        Cpf = signUp.Cpf,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        Status = (int)Enums.CustomerStatus.Validation,
                        CustomerType = (int)Enums.CustomerType.Student,
                        Code = Helper.SecurityHelper.HMACSHA1(signUp.Email, signUp.Email + "|" + signUp.Cpf),
                        IdOperation = operation.Id
                    };

                    if (customerRepo.Create(customer, out error))
                    {
                        var sendingBlue = new Integration.SendinBlueHelper();
                        string link = operation.Domain + "#/?c=" + customer.Code;
                        var result = sendingBlue.SendCustomerValidate(customer.Email, link);

                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = customer.Id });
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

                        identity.AddClaim(new Claim("operationId", operation.Id.ToString()));
                        identity.AddClaim(new Claim("Id", customer.Id.ToString()));
                        identity.AddClaim(new Claim("Name", ""));
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
        /// <returns></returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut("CustomerUpdate")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CustomerUpdate([FromBody] CustomerModel customer)
        {
            var cust = customer.GetEntity();
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

            if (customerRepo.Update(cust, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Cliente atualizado com sucesso!" });

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
                        var sendingBlue = new Integration.SendinBlueHelper();
                        var link = "http://hmlrebens.iasdigitalgroup.com/unicap/c=" + user.Code;
                        var result = sendingBlue.SendPasswordRecover(user, link);
                        if (result.Status)
                            return Ok(new JsonModel() { Status = "ok", Message = result.Message });
                        return StatusCode(400, new JsonModel() { Status = "error", Message = result.Message });
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
        [HttpGet("GetWithdrawSummary")]
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
        /// Retorna o histórico de Benefícios
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
        [HttpPost("Withdraw")]
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
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Resgate registrado com sucesso!", Id = draw.Id });
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
        }

        /// <summary>
        /// Retorna o histórico de Resgates
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        /// <response code="200">Retorna o histórico de resgates do cliente, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListWithdraw")]
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
    }
}