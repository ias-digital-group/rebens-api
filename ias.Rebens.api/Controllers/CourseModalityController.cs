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
    [Route("api/CourseModality"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class CourseModalityController : BaseApiController
    {
        private ICourseModalityRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseModalityRepository"></param>
        public CourseModalityController(ICourseModalityRepository courseModalityRepository)
        {
            this.repo = courseModalityRepository;
        }

        /// <summary>
        /// Lista as modalidades conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da Operação</param>
        /// <returns>Lista com as modalidades encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CourseModalityModel>), 200)]
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

                var ret = new ResultPageModel<CourseModalityModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CourseModalityModel>()
                };

                foreach (var item in list.Page)
                    ret.Data.Add(new CourseModalityModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma modalidade de curso
        /// </summary>
        /// <param name="id">Id da modalidade de curso</param>
        /// <returns>Modalidade</returns>
        /// <response code="200">Retorna a modalidade de curso, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CourseModalityModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var item = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (item == null || item.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CourseModalityModel>() { Data = new CourseModalityModel(item) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma modalidade de curso
        /// </summary>
        /// <param name="modality">Modalidade de Curso</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CourseModalityModel modality)
        {
            if (repo.Update(modality.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Modalidade atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma modalidade de curso
        /// </summary>
        /// <param name="modality"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do período criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CourseModalityModel modality)
        {
            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var p = modality.GetEntity();
            if (idOperation.HasValue)
                p.IdOperation = idOperation.Value;
            if (repo.Create(p, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Modalidade de curso criada com sucesso!", Id = p.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma modalidade de curso
        /// </summary>
        /// <param name="id">Id da modalidade de curso a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Modalidade de curso apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}