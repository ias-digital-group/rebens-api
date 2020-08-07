using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/Account")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private IAdminUserRepository repo;
        private IOperationRepository operationRepo;
        private Constant constant;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adminUserRepository"></param>
        /// <param name="operationRepository"></param>
        public AccountController(IAdminUserRepository adminUserRepository, IOperationRepository operationRepository)
        {
            this.repo = adminUserRepository;
            this.operationRepo = operationRepository;
            this.constant = new Constant();
        }

        /// <summary>
        /// Autentica um usuário na api
        /// </summary>
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
        public IActionResult Login([FromBody]LoginModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            var user = repo.ReadByEmail(model.Email, out string error);
            if (user != null && user.Active)
            {
                if (user.CheckPassword(model.Password))
                {
                    var data = GetToken(user, signingConfigurations, tokenConfigurations);
                    repo.SetLastLogin(user.Id, out error);

                    return Ok(data);
                }
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "O login ou a senha não conferem!" });
        }
        
        /// <summary>
        /// Valida se um token ainda é válido
        /// </summary>
        /// <param name="model"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CheckToken")]
        public IActionResult CheckToken([FromBody]TokenModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            JsonModel resultModel;

            if (model == null || string.IsNullOrEmpty(model.accessToken))
                resultModel = new JsonModel() { Status = "error", Message = "no token" };
            else
            {

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = signingConfigurations.Key,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = tokenConfigurations.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = tokenConfigurations.Issuer
                };

                try
                {
                    var claimsPrincipal = new JwtSecurityTokenHandler()
                        .ValidateToken(model.accessToken, validationParameters, out var rawValidatedToken);

                    if (((JwtSecurityToken)rawValidatedToken).ValidTo > DateTime.UtcNow.AddHours(4))
                        resultModel = new JsonModel() { Status = "ok" };
                    else
                        resultModel = new JsonModel() { Status = "error", Message = "expiring in less than 4 hours" };

                    //return (JwtSecurityToken)rawValidatedToken;
                    // Or, you can return the ClaimsPrincipal
                    // (which has the JWT properties automatically mapped to .NET claims)
                }
                catch (SecurityTokenValidationException stvex)
                {
                    // The token failed validation!
                    // TODO: Log it or display an error.
                    //throw new Exception($"Token failed validation: {stvex.Message}");
                    resultModel = new JsonModel() { Status = "error", Message = "not valid", Data = stvex.Message };
                }
                catch (ArgumentException argex)
                {
                    // The token was not well-formed or was invalid for some other reason.
                    // TODO: Log it or display an error.
                    //throw new Exception($"Token was invalid: {argex.Message}");
                    resultModel = new JsonModel() { Status = "error", Message = "wrong format", Data = argex.Message };
                }

            }
            return Ok(resultModel);
        }

        /// <summary>
        /// Altera a senha do usuário
        /// </summary>
        /// <param name="model">{ Id: id do usuário, OldPassword: senha antiga, NewPassword: nova senha, NewPasswordConfirm: confirmação da nova senha }</param>
        /// <returns></returns>
        /// <respons code="200"></respons>
        /// <respons code="400"></respons>
        [HttpPost("ChangePassword"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens,partnerAdministrator,promoter,couponChecker,ticketChecker,partnerApprover")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            int idAdmin = GetAdminUserId(out string errorId);
            if (!string.IsNullOrEmpty(errorId))
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var user = repo.Read(idAdmin, out string error);
            if(user != null)
            {
                if(user.CheckPassword(model.OldPassword))
                {
                    if(model.NewPassword == model.NewPasswordConfirm)
                    {
                        var salt = Helper.SecurityHelper.GenerateSalt();
                        var encryptedPassword = Helper.SecurityHelper.EncryptPassword(model.NewPassword, salt);
                        if (repo.ChangePassword(idAdmin, encryptedPassword, salt, out error))
                            return Ok(new JsonModel() { Status = "ok" });
                        
                        return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                    }
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "A nova senha e a confirmação da nova senha devem ser iguais!" });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "A senha atual não confere!" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = string.IsNullOrEmpty(error) ? "Usuário não encontrado" : error });
        }

        /// <summary>
        /// Lembrete de senha
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        /// <respons code="200"></respons>
        /// <respons code="400"></respons>
        [AllowAnonymous]
        [HttpGet("RememberPassword")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RememberPassword([FromQuery]string email)
        {
            var user = repo.ReadByEmail(email, out string error);
            if (user != null)
            {
                var code = HttpUtility.UrlEncode(Helper.SecurityHelper.SimpleEncryption(user.Email));
                string body = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {user.Name}.</p> <br /><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Clique no botão <b>“Alterar Senha”</b>  para cadastrar uma nova senha.</p>";
                body += $"<br /><br /><p style=\"text-align:center;\"><a href=\"{constant.AppSettings.App.URL}#/validate?c={code}\" target=\"_blank\" style=\"display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#08061e;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;\">ALTERAR SENHA</a></p>";

                //body += $"<br /><p><a href='{Constant.URL}#/validate?c={code}'>{Constant.URL}#/validate?c={code}</a></p>";
                var listDestinataries = new Dictionary<string, string>() { { user.Email, user.Name } };
                if (Helper.EmailHelper.SendAdminEmail(listDestinataries, "Rebens - Redefinição de senha de cadastro", body, out error))
                    return Ok(new JsonModel() { Status = "ok" });
                return StatusCode(400, new JsonModel() { Status = "ok", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "ok", Message = "Ocorreu um erro ao tentar reenviar a senha!" });
        }

        /// <summary>
        /// Validação de cadastro
        /// </summary>
        /// <param name="model"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Validate")]
        [ProducesResponseType(typeof(TokenModel), 201)]
        [ProducesResponseType(typeof(JsonModel), 404)]
        public IActionResult Validate([FromBody]ValidateModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            var email = Helper.SecurityHelper.SimpleDecryption(model.Code);
            if(!string.IsNullOrEmpty(email))
            {
                var user = repo.ReadByEmail(email, out string error);
                if (user != null)
                {
                    if(model.Password != model.PasswordConfirm)
                        return NotFound(new JsonModel() { Status = "error", Message = "A senha e a confirmação da senha devem ser iguais!" });

                    user.SetPassword(model.Password);
                    if(repo.ChangePassword(user.Id, user.EncryptedPassword, user.PasswordSalt, out error))
                    {
                        var data = GetToken(user, signingConfigurations, tokenConfigurations);
                        repo.SetLastLogin(user.Id, out error);

                        return Ok(data);
                    }
                }
            }

            return NotFound(new JsonModel() { Status = "error", Message = "O código de validação não conferem!" });
        }

        private TokenModel GetToken(AdminUser user, helper.SigningConfigurations signingConfigurations, helper.TokenOptions tokenConfigurations)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(user.Email),
                        new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
                        }
                    );

            var roles = user.Roles.Split(',');
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            string modules = "";
            if (user.IdOperation.HasValue)
                modules = operationRepo.LoadModulesNames(user.IdOperation.Value, out _);

            identity.AddClaim(new Claim("operationPartnerId", user.IdOperationPartner.HasValue ? user.IdOperationPartner.Value.ToString() : "0"));
            identity.AddClaim(new Claim("operationId", user.IdOperation.HasValue ? user.IdOperation.Value.ToString() : "0"));
            identity.AddClaim(new Claim("partnerId", user.IdPartner.HasValue ? user.IdPartner.Value.ToString() : "0"));
            identity.AddClaim(new Claim("id", user.Id.ToString()));
            identity.AddClaim(new Claim("name", user.Name));
            identity.AddClaim(new Claim("surname", user.Surname));
            identity.AddClaim(new Claim("picture", user.Picture ?? ""));
            identity.AddClaim(new Claim("initials", ($"{user.Name.Substring(0, 1)}{(user.Surname.Length > 0 ? user.Surname.Substring(0, 1) : "")}")));
            identity.AddClaim(new Claim("modules", modules));
            if (Enum.TryParse(user.Roles, out Enums.Roles role1))
                identity.AddClaim(new Claim("roleName", Enums.EnumHelper.GetEnumDescription(role1)));

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

            return new TokenModel()
            {
                authenticated = true,
                created = dataCriacao,
                expiration = dataExpiracao,
                accessToken = token
            };
        }
    }
}
 