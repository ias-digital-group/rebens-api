using System.Collections.Generic;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAdminUserRepository repo;

        /// <summary>
        /// User Controller Constructor that receive an Dependency Injection of the Repository
        /// </summary>
        /// <param name="adminUserRepository"></param>
        public UserController(IAdminUserRepository adminUserRepository)
        {
            this.repo = adminUserRepository;
        }

        /// <summary>
        /// Retorna uma lista de usuários conforme os parametros
        /// </summary>
        /// <param name="operationId">id da operação (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os usuários encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", int? operationId = null)
        {
            var list = repo.ListPage(operationId, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<AdminUserModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<AdminUserModel>();
                foreach (var addr in list.Page)
                    ret.Data.Add(new AdminUserModel(addr));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o usuário conforme o ID
        /// </summary>
        /// <param name="id">Id do usuário desejada</param>
        /// <returns>Endereço</returns>
        /// <response code="200">Retorna o usuário, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var addr = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (addr == null || addr.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<AdminUserModel>() { Data = new AdminUserModel(addr) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] AdminUserModel user)
        {
            var model = new JsonModel();

            if (repo.Update(user.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id do usuário criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] AdminUserModel user)
        {
            var adminUser = user.GetEntity();

            if (repo.Create(adminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Usuário criado com sucesso!", Id = adminUser.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}