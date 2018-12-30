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
        /// <summary>
        /// Autentica um usuário na api
        /// </summary>
        /// <param name="model"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public JsonResult Login([FromBody]LoginModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            JsonModel resultModel = new JsonModel();
            var repo = ServiceLocator<IAdminUserRepository>.Create();
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

                    resultModel = new JsonModel()
                    {
                        Status = "ok",
                        Extra = new
                        {
                            token = new TokenModel()
                            {
                                authenticated = true,
                                created = dataCriacao,
                                expiration = dataExpiracao,
                                accessToken = token
                            },
                            user = new { id = user.Id, name = user.Name, email = user.Email }
                        }
                    };
                }
                else
                {
                    resultModel.Status = "error";
                    resultModel.Message = "O login ou a senha não conferem!";
                }
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = "O login ou a senha não conferem!";
            }

            return new JsonResult(resultModel);
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
        public JsonResult CheckToken([FromBody]TokenModel model, [FromServices]helper.SigningConfigurations signingConfigurations, [FromServices]helper.TokenOptions tokenConfigurations)
        {
            JsonModel resultModel;

            if (model == null || string.IsNullOrEmpty(model.accessToken))
                resultModel = new Models.JsonModel() { Status = "error", Message = "no token" };
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
                        resultModel = new Models.JsonModel() { Status = "ok" };
                    else
                        resultModel = new Models.JsonModel() { Status = "error", Message = "expiring in less than 4 hours" };

                    //return (JwtSecurityToken)rawValidatedToken;
                    // Or, you can return the ClaimsPrincipal
                    // (which has the JWT properties automatically mapped to .NET claims)
                }
                catch (SecurityTokenValidationException stvex)
                {
                    // The token failed validation!
                    // TODO: Log it or display an error.
                    //throw new Exception($"Token failed validation: {stvex.Message}");
                    resultModel = new Models.JsonModel() { Status = "error", Message = "not valid", Extra = stvex.Message };
                }
                catch (ArgumentException argex)
                {
                    // The token was not well-formed or was invalid for some other reason.
                    // TODO: Log it or display an error.
                    //throw new Exception($"Token was invalid: {argex.Message}");
                    resultModel = new Models.JsonModel() { Status = "error", Message = "wrong format", Extra = argex.Message };
                }

            }
            return new JsonResult(resultModel);
        }

        //[HttpPost("ChangePassword")]
        //[Authorize("Bearer")]
        //public JsonResult ChangePassword([FromBody]Models.ChangePasswordModel model)
        //{
        //    Models.JsonModel resultModel;

        //    var salt = Helper.SecurityHelper.GenerateSalt();
        //    var encryptedPassword = Helper.SecurityHelper.EncryptPassword(model.Password, salt);
        //    var repo = new UserRepository();
        //    if (repo.ChangePassword(model.Id, salt, encryptedPassword, out string error))
        //        resultModel = new Models.JsonModel() { Status = "ok" };
        //    else
        //        resultModel = new Models.JsonModel() { Status = "error", Message = error };

        //    return new JsonResult(resultModel);
        //}

        //[AllowAnonymous]
        //[HttpPut]
        //public JsonResult Put([FromBody]Models.CreateAccountModel model)
        //{
        //    Models.JsonModel resultModel;

        //    var user = new User();
        //    user.Name = model.Name;
        //    user.Email = model.Email;
        //    user.Status = (int)Enums.UserStatus.New;
        //    user.Created = user.Modified = DateTime.UtcNow;
        //    user.Verified = user.Private = false;
        //    if (!string.IsNullOrEmpty(model.Password))
        //        user.SetPassword(model.Password);

        //    user.fbAccessToken = model.fbToken;
        //    user.fbUserId = model.fbUserId;
        //    user.Picture = model.picture;

        //    var repo = new UserRepository();
        //    if (repo.Create(user, out string error))
        //    {
        //        model.Id = user.Id;
        //        resultModel = new Models.JsonModel() { Status = "ok", Extra = model };
        //    }
        //    else
        //        resultModel = new Models.JsonModel() { Status = "error", Message = error };

        //    return new JsonResult(resultModel);
        //}

        //[AllowAnonymous]
        //[HttpPost("CheckUsername")]
        //public JsonResult CheckUsername([FromBody]Models.CheckUsernameModel model)
        //{
        //    var repo = new UserRepository();
        //    var result = new Models.JsonModel();
        //    result.Status = repo.CheckUserNameAvailable(model.Id, model.Username) ? "ok" : "nok";
        //    return new JsonResult(result);
        //}

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