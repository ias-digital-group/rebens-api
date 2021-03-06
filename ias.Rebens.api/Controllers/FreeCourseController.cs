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
    public class FreeCourseController : BaseApiController
    {
        private IFreeCourseRepository repo;
        private ICategoryRepository categoryRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="freeCourseRepository">Injeção de dependencia do repositório de Curso Livres</param>
        public FreeCourseController(IFreeCourseRepository freeCourseRepository, ICategoryRepository categoryRepository)
        {
            this.repo = freeCourseRepository;
            this.categoryRepo = categoryRepository;
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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var part = course.GetEntity();
            part.IdAdminUser = idAdminUser;
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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            string error = null;
            var item = course.GetEntity();
            item.IdAdminUser = idAdminUser;
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
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
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

        /// <summary>
        /// Lista os ids das categorias vínculadas ao curso
        /// </summary>
        /// <param name="id">id do curso livre</param>
        /// <returns>Lista com os Ids das categorias</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Category")]
        [ProducesResponseType(typeof(JsonDataModel<List<int>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCategories(int id)
        {
            var list = categoryRepo.ListByFreeCourse(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<int>>()
                {
                    Data = list
                };

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona uma categoria a um curso livre
        /// </summary>
        /// <param name="model">{ IdFreeCourse: 0, IdCategory: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um curso livre com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddCategory")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddCategory([FromBody]FreeCourseCategoryModel model)
        {
            if (repo.AddCategory(model.IdFreeCourse, model.IdCategory, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria adicionada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona categorias a um curso livre
        /// </summary>
        /// <param name="model">{ IdFreeCourse: 0, IdCategory: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um curso livre com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SaveCategories")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveCategories([FromBody]FreeCourseCategoriesModel model)
        {
            if (repo.SaveCategories(model.IdFreeCourse, model.CategoryIds, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categorias salvas com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove uma categoria de um curso livre
        /// </summary>
        /// <param name="id">id do curso livre</param>
        /// <param name="idCategory">id da categoria</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Category/{idCategory}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveCategory(int id, int idCategory)
        {
            if (repo.DeleteCategory(id, idCategory, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria removida com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa ou inativa o Curso Livres
        /// </summary>
        /// <param name="id">id do curso livre</param>
        /// <param name="active">true para ativar - false para inativar</param>
        /// <returns>JsonModel</returns>
        /// <response code="200"></response>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPost("ChangeActive/{id}/{active}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ChangeActive(int id, bool active)
        {
            if (repo.ChangeActive(id, active, out string error))
            {
                return Ok(new JsonModel() { Status = "ok" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Duplica um Curso Livre
        /// </summary>
        /// <param name="id">id do curso livre</param>
        /// <returns>JsonModel</returns>
        /// <response code="200"></response>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPost("Duplicate/{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Duplicate(int id)
        {
            if (repo.Duplicate(id, out int newId, out string error))
            {
                return Ok(new JsonModel() { Status = "ok", Message = $"{newId}" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}