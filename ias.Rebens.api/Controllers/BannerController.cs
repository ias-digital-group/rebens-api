using System.Collections.Generic;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Banner Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher")]
    [ApiController]
    public class BannerController : ControllerBase
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
        /// <returns>Lista com os banners encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<BannerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            int? idOperation = null;
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator"))
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

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BannerModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BannerModel>();
                foreach (var banner in list.Page)
                    ret.Data.Add(new BannerModel(banner));

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
            var model = new JsonModel();

            if (repo.Update(banner.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Banner atualizado com sucesso!" });

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
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator"))
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

            var model = new JsonModel();

            var b = banner.GetEntity();
            if (repo.Create(b, out string error))
            {
                if(repo.AddOperation(b.Id, idOperation.Value, out error ))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Banner criado com sucesso!", Id = b.Id });
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
            var model = new JsonModel();

            if (repo.Delete(id, out string error))
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
            var resultModel = new JsonModel();

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
            var resultModel = new JsonModel();

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

    }
}