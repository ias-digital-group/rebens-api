using System;
using System.Collections.Generic;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Category Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryRepository repo;
        private ILogErrorRepository logRepo;
        private IOperationRepository operationRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryRepository"></param>
        /// <param name="logErrorRepository"></param>
        /// <param name="operationRepository"></param>
        public CategoryController(ICategoryRepository categoryRepository, ILogErrorRepository logErrorRepository, IOperationRepository operationRepository) {
            this.repo = categoryRepository;
            this.logRepo = logErrorRepository;
            this.operationRepo = operationRepository;
        }

        /// <summary>
        /// Retorna uma lista de categorias conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <param name="idParent">id do pai, não obrigatório (default=null)</param>
        /// <param name="type">tipo de categoria, obrigatório (1=beneficios, 2=cursos livres)</param>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "master,administrator,publiser,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<CategoryModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int type, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", 
            [FromQuery]bool? active = null, [FromQuery]int? idParent = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, type, out string error, active, idParent);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CategoryModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CategoryModel>()
                };
                foreach (var cat in list.Page)
                    ret.Data.Add(new CategoryModel(cat));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a categoria conforme o ID
        /// </summary>
        /// <param name="id">Id da categoria desejada</param>
        /// <returns>Categoria</returns>
        /// <response code="200">Retorna a categoria, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,publisherRebens,administratorRebens")]
        [ProducesResponseType(typeof(JsonDataModel<CategoryModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var category = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (category == null || category.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CategoryModel>() { Data = new CategoryModel(category) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma categoria
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "master,publisherRebens,administratorRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CategoryModel category)
        {
            var model = new JsonModel();
            if (repo.Update(category.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma categoria
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da categoria criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master,publisherRebens,administratorRebens")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CategoryModel category)
        {
            var model = new JsonModel();
            var cat = category.GetEntity();
            if(repo.Create(cat, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Categoria criada com sucesso!", Id = cat.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma categoria
        /// </summary>
        /// <param name="id">Id da categoria a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}"), Authorize("Bearer", Roles = "master,publisherRebens,administratorRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a arvore de categorias 
        /// </summary>
        /// <param name="operationCode">código da operação, obrigatório</param>
        /// <param name="type">tipo de categoria (1 = benefícios, 2 = Cursos Lirves), obrigatório</param>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListTree")]
        [ProducesResponseType(typeof(JsonDataModel<List<CategoryModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListTree([FromHeader(Name = "x-operation-code")]string operationCode, [FromQuery]int type)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);
            int idOperation = 0;
            string error;

            var principal = HttpContext.User;
            if (operationGuid == Guid.Empty)
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
                }
            }
            else
            {
                var operation = operationRepo.Read(operationGuid, out error);
                idOperation = operation.Id;
            }
            if (idOperation == 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            bool isCustomer = (operationGuid != Guid.Empty || principal.IsInRole("customer"));

            var list = repo.ListTree(type, isCustomer, idOperation, out error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<CategoryModel>>();
                ret.Data = new List<CategoryModel>();
                list.ForEach(item => { ret.Data.Add(new CategoryModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a arvore de categorias 
        /// </summary>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListTreeAdm"), Authorize("Bearer", Roles = "master,administrator,publiser,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<List<CategoryModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListTreeAdm([FromQuery]int type)
        {
            var list = repo.ListTree(type, false, null, out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<CategoryModel>>();
                ret.Data = new List<CategoryModel>();
                list.ForEach(item => { ret.Data.Add(new CategoryModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
