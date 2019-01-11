using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
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

        public HelperController(IBenefitTypeRepository benefitTypeRepository, IIntegrationTypeRepository integrationTypeRepository, IOperationTypeRepository operationTypeRepository, IStaticTextTypeRepository staticTextTypeRepository)
        {
            this.benefitRepo = benefitTypeRepository;
            this.integrationRepo = integrationTypeRepository;
            this.operationRepo = operationTypeRepository;
            this.staticTextRepo = staticTextTypeRepository;
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

                return Ok(new { Data = ret });
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

                return Ok(new { Data = ret });
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

                return Ok(new { Data = ret });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}