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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,administratorRebens,")]
    [ApiController]
    public class WirecardController : ControllerBase
    {
        private readonly IMoipRepository repo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="moipRepository"></param>
        public WirecardController(IMoipRepository moipRepository)
        {
            this.repo = moipRepository;
        }

        /// <summary>
        /// Retorna uma lista com as assinaturas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageItems"></param>
        /// <param name="word"></param>
        /// <param name="idOperation"></param>
        /// <returns>Lista com as assinaturas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<MoipSignatureModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 20, [FromQuery] string word = null, [FromQuery]int? idOperation = null)
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                if (principal.IsInRole("administrator"))
                {
                    var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (tmpId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });
                    if (int.TryParse(tmpId.Value, out int tmpIdOperation))
                        idOperation = tmpIdOperation;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrado!" });
                }
            }

            ResultPage<MoipSignature> list = repo.ListSubscriptions(page, pageItems, word, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<MoipSignatureModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<MoipSignatureModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new MoipSignatureModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

    }
}
