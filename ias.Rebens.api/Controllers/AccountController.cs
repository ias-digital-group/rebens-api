using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
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
    [Route("api/Account"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAdminUserRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adminUserRepository"></param>
        public AccountController(IAdminUserRepository adminUserRepository)
        {
            this.repo = adminUserRepository;
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

                    var roles = user.Roles.Split(',');
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }

                    //foreach (var policy in user.Permissions)
                    //    identity.AddClaim(new Claim("permissions", "permission1"));

                    identity.AddClaim(new Claim("operationId", user.IdOperation.HasValue ? user.IdOperation.Value.ToString() : "0"));
                    identity.AddClaim(new Claim("Id", user.Id.ToString()));
                    identity.AddClaim(new Claim("Name", user.Name));

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

                    repo.SetLastLogin(user.Id, out error);

                    return Ok(Data);
                }
            }

            return NotFound(new JsonModel() { Status = "error", Message = "O login ou a senha não conferem!" });

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
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            var user = repo.Read(model.Id, out string error);
            if(user != null)
            {
                if(user.CheckPassword(model.OldPassword))
                {
                    if(model.NewPassword == model.NewPasswordConfirm)
                    {
                        var salt = Helper.SecurityHelper.GenerateSalt();
                        var encryptedPassword = Helper.SecurityHelper.EncryptPassword(model.NewPassword, salt);
                        if (repo.ChangePassword(model.Id, encryptedPassword, salt, out error))
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
                string password = Helper.SecurityHelper.CreatePassword();
                user.SetPassword(password);
                if (repo.ChangePassword(user.Id, user.PasswordSalt, user.EncryptedPassword, out string errorSave))
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
    }
}
 