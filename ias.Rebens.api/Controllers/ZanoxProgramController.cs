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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
    [ApiController]
    public class ZanoxProgramController : BaseApiController
    {
        private IZanoxProgramRepository repo;

        public ZanoxProgramController(IZanoxProgramRepository zanoxProgram)
        {
            repo = zanoxProgram;
        }

        /// <summary>
        /// Lista os Programas da Zanox
        /// </summary>
        /// <param name="page">pagina (default = 0)</param>
        /// <param name="pageItems">itens por página (default = 30)</param>
        /// <param name="searchWord">palavra à ser buscada</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<ZanoxProgramModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 30, [FromQuery] string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ZanoxProgramModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ZanoxProgramModel>()
                };
                foreach (var item in list.Page)
                    ret.Data.Add(new ZanoxProgramModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um programa
        /// </summary>
        /// <param name="id">Id do programa desejada</param>
        /// <returns>programa</returns>
        /// <response code="200">Retorna o programa, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<ZanoxProgramModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var program = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (program == null || program.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<ZanoxProgramModel>() { Data = new ZanoxProgramModel(program) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um programa
        /// </summary>
        /// <param name="program">Programa</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] ZanoxProgramModel program)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Save(program.GetEntity(), out string error, idAdminUser))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Programa atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Altera o status da publicação de um programa
        /// </summary>
        /// <param name="id">id do programa</param>
        /// <returns></returns>
        /// <response code="200">Se o tudo ocorrer sem erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/TogglePublish")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult TogglePublish(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var status = repo.TogglePublish(id, idAdminUser, out string error);

            if (string.IsNullOrEmpty(error))
                return Ok(new JsonModel() { Status = "ok", Data = status });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
