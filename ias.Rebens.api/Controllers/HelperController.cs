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
        /// <response code="201">Retorna a lista, ou algum erro, caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("ListBenefitType")]
        public IActionResult ListBenefitType()
        {
            var list = benefitRepo.ListActive(out string error);

            
            if (string.IsNullOrEmpty(error))
            {
                if (list != null && list.Count() == 0)
                    return NoContent();

                var ret = new List<BenefitTypeModel>();
                list.ForEach(item => { ret.Add(new BenefitTypeModel(item)); });

                return Ok(new { Data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de integração
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("ListIntegrationType")]
        public IActionResult ListIntegrationType()
        {
            var list = integrationRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list != null && list.Count() == 0)
                    return NoContent();

                var ret = new List<IntegrationTypeModel>();
                list.ForEach(item => { ret.Add(new IntegrationTypeModel(item)); });

                return Ok(new { Data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de operação
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("ListOperationType")]
        public IActionResult ListOperationType()
        {
            var list = operationRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list != null && list.Count() == 0)
                    return NoContent();

                var ret = new List<OperationTypeModel>();
                list.ForEach(item => { ret.Add(new OperationTypeModel(item)); });

                return Ok(new { Data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os tipos de textos estáticos
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("ListStaticTextType")]
        public IActionResult ListStaticTextType()
        {
            var list = staticTextRepo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list != null && list.Count() == 0)
                    return NoContent();

                var ret = new List<StaticTextTypeModel>();
                list.ForEach(item => { ret.Add(new StaticTextTypeModel(item)); });

                return Ok(new { Data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }
    }
}