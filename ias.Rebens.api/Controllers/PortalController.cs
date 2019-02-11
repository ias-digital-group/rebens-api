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
    [Route("api/[controller]"), Authorize("Bearer")]
    [ApiController]
    public class PortalController : ControllerBase
    {
        private IAddressRepository addrRepo;
        private IBannerRepository bannerRepo;
        private IBenefitRepository benefitRepo;
        private IFaqRepository faqRepo;
        private IFormContactRepository formContactRepo;
        private IFormEstablishmentRepository formEstablishmentRepo;
        private IOperationRepository operationRepo;
        private ICustomerRepository customerRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bannerRepository"></param>
        /// <param name="benefitRepository"></param>
        /// <param name="faqRepository"></param>
        /// <param name="formContactRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="formEstablishmentRepository"></param>
        /// <param name="customerRepository"></param>
        public PortalController(IBannerRepository bannerRepository, IBenefitRepository benefitRepository, IFaqRepository faqRepository, IFormContactRepository formContactRepository, IOperationRepository operationRepository, IFormEstablishmentRepository formEstablishmentRepository, ICustomerRepository customerRepository)
        {
            this.bannerRepo = bannerRepository;
            this.benefitRepo = benefitRepository;
            this.faqRepo = faqRepository;
            this.formContactRepo = formContactRepository;
            this.operationRepo = operationRepository;
            this.formEstablishmentRepo = formEstablishmentRepository;
            this.customerRepo = customerRepository;
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
        /// <respons code="404">se não encontrar o usuário ou a senha não estiver correta</respons>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(TokenModel), 201)]
        [ProducesResponseType(typeof(JsonModel), 404)]
        public IActionResult Login([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody]LoginModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var user = customerRepo.ReadByEmail(model.Email, operation.Id, out error);
                if (user != null)
                {
                    if (user.CheckPassword(model.Password))
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

                        identity.AddClaim(new Claim("operationId", user.IdOperation.ToString()));
                        identity.AddClaim(new Claim("Id", user.Id.ToString()));
                        identity.AddClaim(new Claim("Name", user.Name));
                        identity.AddClaim(new Claim("Email", user.Email));

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

                        var Data = new TokenModel()
                        {
                            authenticated = true,
                            created = dataCriacao,
                            expiration = dataExpiracao,
                            accessToken = token
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
        /// <param name="idBenefitType">tipo de benefício, não obrigatório (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Benefits")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefits([FromQuery]int? idCategory = null, [FromQuery]int? idBenefitType = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
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

            var list = benefitRepo.ListByOperation(idOperation, idCategory, idBenefitType, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BenefitModel>();
                foreach (var benefit in list.Page)
                    ret.Data.Add(new BenefitModel(benefit));

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
        /// <param name="customer"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpPost("CustomerCreate")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CustomerCreate([FromBody]CustomerModel customer)
        {
            var cust = customer.GetEntity();
            string error = null;

            if (cust.Address != null)
            {
                var addr = customer.Address.GetEntity();
                if (addrRepo.Create(addr, out error))
                    cust.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            if (customerRepo.Create(cust, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = cust.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
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
                if (addrRepo.Update(addr, out error))
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
            var customer = customerRepo.Read(model.Id, out string error);
            if (customer != null)
            {
                if (customer.CheckPassword(model.OldPassword))
                {
                    if (model.NewPassword == model.NewPasswordConfirm)
                    {
                        var salt = Helper.SecurityHelper.GenerateSalt();
                        var encryptedPassword = Helper.SecurityHelper.EncryptPassword(model.NewPassword, salt);
                        if (customerRepo.ChangePassword(model.Id, encryptedPassword, salt, out error))
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
        [HttpGet("RememberPassword")]
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
                    string password = Helper.SecurityHelper.CreatePassword();
                    user.SetPassword(password);
                    if (customerRepo.ChangePassword(user.Id, user.PasswordSalt, user.EncryptedPassword, out string errorSave))
                    {
                        var sendingBlue = new Integration.SendinBlueHelper();
                        string body = "<p><b>Nova Senha:</b>" + password + "</p>";
                        var result = sendingBlue.Send(user.Email, user.Name, "contato@rebens.com.br", "Contato", "[Rebens] - Lembrete de Senha", body);
                        if (result.Status)
                            return Ok(new JsonModel() { Status = "ok", Message = result.Message });
                        return StatusCode(400, new JsonModel() { Status = "ok", Message = result.Message });
                    }
                    return StatusCode(400, new JsonModel() { Status = "ok", Message = "Ocorreu um erro ao tentar reenviar a senha!" });
                }
                return StatusCode(400, new JsonModel() { Status = "ok", Message = "Ocorreu um erro ao tentar reenviar a senha!" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

        }
    }
}