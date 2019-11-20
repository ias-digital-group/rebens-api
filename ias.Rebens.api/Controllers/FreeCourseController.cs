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
    /// <summary>
    /// FreeCourse Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class FreeCourseController : ControllerBase
    {
        private IFreeCourseRepository repo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="freeCourseRepository">Injeção de dependencia do repositório de Curso Livres</param>
        public FreeCourseController(IFreeCourseRepository freeCourseRepository)
        {
            this.repo = freeCourseRepository;

        }

        /// <summary>
        /// Retorna o curso conforme o ID
        /// </summary>
        /// <param name="id">Id do curso</param>
        /// <returns>Curso</returns>
        /// <response code="200">Retorna o curso, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<FreeCourseModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var course = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (course == null || course.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<FreeCourseModel>() { Data = new FreeCourseModel(course) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]FreeCourseModel course)
        {
            var principal = HttpContext.User;
            var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
            if (customerId == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            if (!int.TryParse(customerId.Value, out int idAdmin))
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            var part = course.GetEntity();
            part.IdAdminUser = idAdmin;
            if (repo.Update(part, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Curso atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do curso criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]FreeCourseModel course)
        {
            int? idOperation = null;
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
            if (customerId == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            if (!int.TryParse(customerId.Value, out int idAdmin))
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            string error = null;
            var model = new JsonModel();

            var item = course.GetEntity();
            item.IdAdminUser = idAdmin;
            if (idOperation.HasValue)
                item.IdOperation = idOperation.Value;

            if (repo.Create(item, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Curso criado com sucesso!", Id = item.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o curso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Curso removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todos os cursos com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">id da operação, não obrigatório (default=null)</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <returns>Lista com os cursos encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<FreeCourseItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "",
            [FromQuery]int? idOperation = null, [FromQuery]bool? active = null)
        {
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation: idOperation, status: active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<FreeCourseItemModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<FreeCourseItemModel>()
                };
                foreach (var item in list.Page)
                    ret.Data.Add(new FreeCourseItemModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "" });
        }
    }
}