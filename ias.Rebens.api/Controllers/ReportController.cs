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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator,master")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportRepository repo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportRepository"></param>
        public ReportController(IReportRepository reportRepository)
        {
            this.repo = reportRepository;
        }

        /// <summary>
        /// Método que retorna as informações necessárias para montar o dashboard do admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoadDashboard")]
        [ProducesResponseType(typeof(JsonDataModel<Dashboard>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult LoadDashboard()
        {
            int? idOperation = null;
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out int tmpIdOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    idOperation = tmpIdOperation;
                }
            }

            var ret = new JsonDataModel<Dashboard>();
            ret.Data = repo.LoadDashboard(out string error, null, null, idOperation);
            if (string.IsNullOrEmpty(error))
                return Ok(ret);

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}