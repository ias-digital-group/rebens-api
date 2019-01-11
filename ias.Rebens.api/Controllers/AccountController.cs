using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAdminUserRepository repo;

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
        [ProducesResponseType(typeof(LoginResultModel), 201)]
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

                    //foreach (var role in user.Roles)
                    //{
                    //identity.AddClaim(new Claim(ClaimTypes.Role, "test"));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "administrator"));
                    //}

                    //foreach (var policy in user.Permissions)
                    //{
                    //    identity.AddClaim(new Claim("permissions", "permission1"));
                    //}

                    identity.AddClaim(new Claim("operationId", "77"));
                    identity.AddClaim(new Claim("Id", user.Id.ToString()));

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

                    var Data = new LoginResultModel()
                    {
                        Token = new TokenModel()
                        {
                            authenticated = true,
                            created = dataCriacao,
                            expiration = dataExpiracao,
                            accessToken = token
                        },
                        User = new UserModel(){ Id = user.Id, Name = user.Name, Email = user.Email },
                        Role = "administrator"
                    };

                    repo.SetLastLogin(user.Id, out error);

                    return Ok(Data);
                }
            }

            var resultModel = new JsonModel()
            {
                Status = "error",
                Message = "O login ou a senha não conferem!"
            };
            return NotFound(resultModel);
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
        [HttpPost("ChangePassword")]
        [Authorize("Bearer")]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            JsonModel resultModel;
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
                            resultModel = new JsonModel() { Status = "ok" };
                        else
                            resultModel = new JsonModel() { Status = "error", Message = error };
                    }
                    else
                        resultModel = new JsonModel() { Status = "error", Message = "A nova senha e a confirmação da nova senha devem ser iguais!" };
                }
                else
                    resultModel = new JsonModel() { Status = "error", Message = "A senha atual não confere!" };
            }
            else
                resultModel = new JsonModel() { Status = "error", Message = string.IsNullOrEmpty(error) ? "Usuário não encontrado" : error };
            
            return Ok(resultModel);
        }

        //[AllowAnonymous]
        //[HttpGet("RememberPassword")]
        //public JsonResult RememberPassword([FromQuery]string email)
        //{
        //    var repo = new UserRepository();
        //    var result = new JsonModel();
        //    result.Status = "error";
        //    var tmp = repo.ReadByLogin(email, out string error);
        //    if (tmp != null)
        //    {
        //        string password = Helper.SecurityHelper.CreatePassword();
        //        tmp.SetPassword(password);
        //        if (repo.ChangePassword(tmp.Id, tmp.PasswordSalt, tmp.EncryptedPassword, out string errorSave))
        //        {
        //            if (Helper.EmailHelper.SendPasswordReminder(tmp, password))
        //                result.Status = "ok";
        //        }
        //        else
        //            result.Message = errorSave;

        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(error))
        //            result.Message = Resources.Resource.UserRepositoryUserNotFound;
        //        else
        //            result.Message = error;
        //    }

        //    return new JsonResult(result);
        //}
    }
}