using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Helper Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Helper controller constructor that receive the dependency injection of the repositories of Hosting Enviroment
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public HelperController(IHostingEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Lista os tipos de benefício
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro, caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListBenefitType")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitType()
        {
            try
            {
                var ret = new JsonDataModel<List<BenefitTypeModel>>();
                ret.Data = new List<BenefitTypeModel>();

                foreach (Enums.BenefitType type in Enum.GetValues(typeof(Enums.BenefitType)))
                {
                    ret.Data.Add(new BenefitTypeModel(type));
                }
                return Ok(ret);
            }
            catch(Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Lista os tipos de integração
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [HttpGet("ListIntegrationType")]
        [ProducesResponseType(typeof(JsonDataModel<List<IntegrationTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListIntegrationType()
        {
            try
            {
                var ret = new JsonDataModel<List<IntegrationTypeModel>>();
                ret.Data = new List<IntegrationTypeModel>();

                foreach (Enums.IntegrationType type in Enum.GetValues(typeof(Enums.IntegrationType)))
                {
                    ret.Data.Add(new IntegrationTypeModel(type));
                }
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Lista os tipos de operação
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [HttpGet("ListOperationType")]
        [ProducesResponseType(typeof(JsonDataModel<List<OperationTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListOperationType()
        {
            try
            {
                var ret = new JsonDataModel<List<OperationTypeModel>>();
                ret.Data = new List<OperationTypeModel>();

                foreach (Enums.OperationType type in Enum.GetValues(typeof(Enums.OperationType)))
                {
                    ret.Data.Add(new OperationTypeModel(type));
                }
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Lista os tipos de textos estáticos
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [HttpGet("ListStaticTextType")]
        [ProducesResponseType(typeof(JsonDataModel<List<StaticTextTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListStaticTextType()
        {
            try
            {
                var ret = new JsonDataModel<List<StaticTextTypeModel>>();
                ret.Data = new List<StaticTextTypeModel>();

                foreach (Enums.StaticTextType type in Enum.GetValues(typeof(Enums.StaticTextType)))
                {
                    ret.Data.Add(new StaticTextTypeModel(type));
                }
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Lista os tipos de banner
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administrator,customer,administratorRebens,publisherRebens")]
        [HttpGet("ListBannerType")]
        [ProducesResponseType(typeof(JsonDataModel<List<BannerTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBannerType()
        {
            try
            {
                var ret = new JsonDataModel<List<BannerTypeModel>>();
                ret.Data = new List<BannerTypeModel>();
                foreach (Enums.BannerType type in Enum.GetValues(typeof(Enums.BannerType)))
                {
                    ret.Data.Add(new BannerTypeModel() { Id = (int)type, Name = Enums.EnumHelper.GetEnumDescription(type) });
                }

                return Ok(ret);
            }
            catch(Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// recebe uma imagem e salva no servidor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se ocorrer algum erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administrator,customer,publisher,administratorRebens,publisherRebens")]
        [HttpPost("UploadImage"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(FileUploadResultModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UploadImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, "files");
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString("n") + extension;
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var cloudinary = new Integration.CloudinaryHelper();
                    var ret = cloudinary.UploadFile(fullPath, "Portal");
                    if(ret.Status)
                        return Ok(new FileUploadResultModel() { FileName = ret.public_id + extension, Url = ret.secure_url });

                    return StatusCode(400, new JsonModel() { Status = "error", Message = ret.Message });
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// recebe um arquivo e salva no servidor
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se ocorrer algum erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,administrator,customer,publisher,administratorRebens,publisherRebens")]
        [HttpPost("UploadFile/{type}"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(FileUploadResultModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UploadFile(string type)
        {
            try
            {
                var file = Request.Form.Files[0];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, "files", type);
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString("n") + extension;
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    var constant = new Constant();
                    return Ok(new FileUploadResultModel() { FileName = fileName, Url = $"{constant.URL}files/{type}/{fileName}" });
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// envia um email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        /// <response code="200">Retorna um modelo, ou algum erro caso interno</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [Authorize("Bearer", Roles = "master,publisher,administrator,customer,administratorRebens,publisherRebens")]
        [HttpPost("SendEmail")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SendEmail([FromBody]EmailModel email)
        {
            var sendingBlue = new Integration.SendinBlueHelper();
            var listDestinataries = new Dictionary<string, string>() { { email.ToEmail, email.ToName } };
            var result = sendingBlue.Send(listDestinataries, email.FromEmail, email.FromName, email.Subject, email.Message);
            if (result.Status)
                return Ok(new JsonModel() { Status = "ok", Message = result.Message });

            return StatusCode(400, new JsonModel() { Status = "error", Message = result.Message });
        }
    }
}