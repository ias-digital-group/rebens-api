using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/CoursePeriod"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class CoursePeriodController : BaseApiController
    {
        private ICoursePeriodRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coursePeriodRepository"></param>
        public CoursePeriodController(ICoursePeriodRepository coursePeriodRepository)
        {
            this.repo = coursePeriodRepository;
        }

        /// <summary>
        /// Lista os periodos conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da Operação</param>
        /// <returns>Lista com os períodos encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CoursePeriodModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null)
        {
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new ResultPageModel<CoursePeriodModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CoursePeriodModel>()
                };

                foreach (var period in list.Page)
                    ret.Data.Add(new CoursePeriodModel(period));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um período de curso
        /// </summary>
        /// <param name="id">Id do período de curso</param>
        /// <returns>FAQ</returns>
        /// <response code="200">Retorna o período do curso, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CoursePeriodModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var period = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (period == null || period.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CoursePeriodModel>() { Data = new CoursePeriodModel(period) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um período de curso
        /// </summary>
        /// <param name="period">Período de Curso</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CoursePeriodModel period)
        {
            if (repo.Update(period.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Período atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um período de curso
        /// </summary>
        /// <param name="period"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do período criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CoursePeriodModel period)
        {
            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var p = period.GetEntity();
            if (idOperation.HasValue)
                p.IdOperation = idOperation.Value;
            if (repo.Create(p, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Período de curso criado com sucesso!", Id = p.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um período de curso
        /// </summary>
        /// <param name="id">Id do período de curso a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Período de curso apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}