using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private IBenefitTypeRepository benefitRepo;
        private IIntegrationTypeRepository integrationRepo;
        private IOperationTypeRepository operationRepo;
        private IStaticTextTypeRepository staticTextRepo;
        private IHostingEnvironment _hostingEnvironment;

        public HelperController(IBenefitTypeRepository benefitTypeRepository, IIntegrationTypeRepository integrationTypeRepository, IOperationTypeRepository operationTypeRepository, IStaticTextTypeRepository staticTextTypeRepository, IHostingEnvironment hostingEnvironment)
        {
            this.benefitRepo = benefitTypeRepository;
            this.integrationRepo = integrationTypeRepository;
            this.operationRepo = operationTypeRepository;
            this.staticTextRepo = staticTextTypeRepository;
            this._hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Lista os tipos de benefício
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro, caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListBenefitType")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitType()
        {
            var list = benefitRepo.ListActive(out string error);
            
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BenefitTypeModel>>();
                ret.Data = new List<BenefitTypeModel>();
                list.ForEach(item => { ret.Data.Add(new BenefitTypeModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de integração
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListIntegrationType")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListIntegrationType()
        {
            var list = integrationRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<IntegrationTypeModel>>();
                ret.Data = new List<IntegrationTypeModel>();
                list.ForEach(item => { ret.Data.Add(new IntegrationTypeModel(item)); });

                return Ok(ret);
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de operação
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListOperationType")]
        [ProducesResponseType(typeof(JsonDataModel<List<OperationTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListOperationType()
        {
            var list = operationRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<OperationTypeModel>>();
                ret.Data = new List<OperationTypeModel>();
                list.ForEach(item => { ret.Data.Add(new OperationTypeModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de textos estáticos
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListStaticTextType")]
        [ProducesResponseType(typeof(JsonDataModel<List<StaticTextTypeModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListStaticTextType()
        {
            var list = staticTextRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<StaticTextTypeModel>>();
                ret.Data = new List<StaticTextTypeModel>();
                list.ForEach(item => { ret.Data.Add(new StaticTextTypeModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de banner
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListBannerType")]
        [ProducesResponseType(typeof(JsonDataModel<List<BannerModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBannerType()
        {
            var ret = new JsonDataModel<List<BannerTypeModel>>();
            ret.Data = new List<BannerTypeModel>();
            ret.Data.Add(new BannerTypeModel() { Id = 1, Name = "Home destaque" });
            ret.Data.Add(new BannerTypeModel() { Id = 2, Name = "Categoria 1" });
            ret.Data.Add(new BannerTypeModel() { Id = 3, Name = "Categorai 2" });

            return Ok(ret);

            //var list = staticTextRepo.ListActive(out string error);

            //if (string.IsNullOrEmpty(error))
            //{
            //    if (list == null || list.Count() == 0)
            //        return NoContent();

            //    var ret = new JsonDataModel<List<BannerModel>>();
            //    ret.Data = new List<StaticTextTypeModel>();
            //    list.ForEach(item => { ret.Data.Add(new StaticTextTypeModel(item)); });

            //    return Ok(new { Data = ret });
            //}

            //return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// recebe um arquivo e salva no servidor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se ocorrer algum erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(FileUploadResultModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UploadFile()
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
                    var ret = cloudinary.UploadFile(fullPath, "rebens");
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
        /// envia um email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        /// <response code="200">Retorna um modelo, ou algum erro caso interno</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SendEmail")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SendEmail([FromBody]EmailModel email)
        {
            var model = new JsonModel();
            var sendingBlue = new Integration.SendinBlueHelper();
            var result = sendingBlue.Send(email.ToEmail, email.ToName, email.FromEmail, email.FromName, email.Subject, email.Message);
            if (result.Status)
                return Ok(new JsonModel() { Status = "ok", Message = result.Message });

            return StatusCode(400, new JsonModel() { Status = "error", Message = result.Message });
        }
    }
}