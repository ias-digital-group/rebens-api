using System.Collections.Generic;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Clauses;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Banner Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class BannerController : BaseApiController
    {
        private IBannerRepository repo;
        private IOperationRepository operationRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bannerRepository"></param>
        /// <param name="operationRepository"></param>
        public BannerController(IBannerRepository bannerRepository, IOperationRepository operationRepository)
        {
            this.repo = bannerRepository;
            this.operationRepo = operationRepository;
        }

        /// <summary>
        /// Lista os banners conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">Active, não obrigatório (default=null)</param>
        /// <param name="type">Tipo de banner, não obrigatório (default=null)</param>
        /// <param name="idOperation">id da Operação, não obrigatório (default=null)</param>
        /// <returns>Lista com os banners encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<BannerListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", 
            [FromQuery]string searchWord = "", [FromQuery]bool? active = null, [FromQuery]int? type = null, [FromQuery]int? idOperation = null, [FromQuery]string where = null)
        {
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation, active, type, where);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BannerListItemModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BannerListItemModel>()
                };
                foreach (var banner in list.Page)
                    ret.Data.Add(new BannerListItemModel(banner));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um banner
        /// </summary>
        /// <param name="id">Id do banner desejada</param>
        /// <returns>Banner</returns>
        /// <response code="200">Retorna o banner, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<BannerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var banner = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (banner == null || banner.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<BannerModel>() { Data = new BannerModel(banner) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um banner
        /// </summary>
        /// <param name="banner">Banner</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] BannerModel banner)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Update(banner.GetEntity(), idAdminUser, out string error))
            {
                if (repo.ConnectOperations(banner.Id, banner.Operations, out error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Banner atualizado com sucesso!" });
                else
                    return Ok(new JsonCreateResultModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um Banner
        /// </summary>
        /// <param name="banner">Banner</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] BannerModel banner)
        {
            int? idOperation = null;
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }
             
            var b = banner.GetEntity();
            if (repo.Create(b, idAdminUser, out string error))
            {
                if (idOperation.HasValue)
                {
                    if (repo.AddOperation(b.Id, idOperation.Value, out error))
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Banner criado com sucesso!", Id = b.Id });
                    else
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = error, Id = b.Id });
                }
                else
                {
                    if(repo.ConnectOperations(b.Id, banner.Operations, out error))
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Banner criado com sucesso!", Id = b.Id });
                    else
                        return Ok(new JsonCreateResultModel() { Status = "error", Message = error, Id = b.Id });
                }
                    
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um banner
        /// </summary>
        /// <param name="id">Id do banner a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Banner apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um banner a uma operação
        /// </summary>
        /// <param name="model">{ IdBanner: 0, IdOperation: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um banner com uma Operação</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddOperation")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddOperation([FromBody]BannerOperationModel model)
        {
            if (repo.AddOperation(model.IdBanner, model.IdOperation, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Operação adicionada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um banner de uma operação
        /// </summary>
        /// <param name="id">id do banner</param>
        /// <param name="idOperation">id da operação</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de banner com uma operação</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Operation/{idOperation}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveOperation(int id, int idOperation)
        {
            if (repo.DeleteOperation(id, idOperation, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Operação removida com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todas operações, e marca quais estão vinculadas ao banner
        /// </summary>
        /// <param name="id">id do banner</param>
        /// <returns>Lista com todas operações</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Operations")]
        [ProducesResponseType(typeof(JsonDataModel<List<BannerOperationItemModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListOperations(int id)
        {
            var list = operationRepo.ListByBanner(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BannerOperationItemModel>>();
                ret.Data = new List<BannerOperationItemModel>();
                foreach (var item in list)
                    ret.Data.Add(new BannerOperationItemModel(item));

                return Ok(ret);
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa/Inativa um banner
        /// </summary>
        /// <param name="id">id do banner</param>
        /// <returns></returns>
        /// <response code="200">Se o tudo ocorrer sem erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/ToggleActive")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ToggleActive(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var status = repo.ToggleActive(id, idAdminUser, out string error);

            if (string.IsNullOrEmpty(error))
                return Ok(new JsonModel() { Status = "ok", Data = status });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}