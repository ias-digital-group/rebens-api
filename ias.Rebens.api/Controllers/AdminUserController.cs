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
    /// AdminUser Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private IAdminUserRepository repo;
        private IOperationRepository opreationRepo;

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="operationRepository"></param>
        /// <param name="adminUserRepository"></param>
        public AdminUserController(IOperationRepository operationRepository, IAdminUserRepository adminUserRepository)
        {
            this.repo = adminUserRepository;
            this.opreationRepo = operationRepository;
        }

        /// <summary>
        /// Retorna uma lista de usuários conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os usuários encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(ResultPageModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(null, page, pageItems, searchWord, sort, out string error);
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
                foreach (var admin in list.Page)
                    ret.Data.Add(new AdminUserModel(admin));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna Usuário conforme o ID
        /// </summary>
        /// <param name="id">Id do usuário desejada</param>
        /// <returns>Categoria</returns>
        /// <response code="200">Retorna o usuário, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(JsonDataModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var admin = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (admin == null || admin.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<AdminUserModel>() { Data = new AdminUserModel(admin) });
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
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]AdminUserModel user)
        {
            var model = new JsonModel();
            var admin = user.GetEntity();
           
            if (repo.Update(admin, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do usuário criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]AdminUserModel user)
        {
            var model = new JsonModel();
            var admin = user.GetEntity();
                
            if (repo.Create(admin, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Usuário criado com sucesso!", Id = admin.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um usuário
        /// </summary>
        /// <param name="id">Id do usuário a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [Authorize("Bearer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}