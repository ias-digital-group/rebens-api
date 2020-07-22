using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// StaticText Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/StaticText"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class StaticTextController : BaseApiController
    {
        private IStaticTextRepository repo;

        /// <summary>
        /// Static Text Controller Construction
        /// </summary>
        /// <param name="staticTextRepository"></param>
        public StaticTextController(IStaticTextRepository staticTextRepository)
        {
            this.repo = staticTextRepository;
        }

        /// <summary>
        /// Lista os textos conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idStaticTextType">Id do tipo de texto estático (default=paginas)</param>
        /// <param name="idOperation">Id da operação (default=null)</param>
        /// <returns>Lista com os textos encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<StaticTextListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", 
            [FromQuery]int idStaticTextType = (int)Enums.StaticTextType.Pages, [FromQuery]int? idOperation = null)
        {
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }
            
            var list = repo.ListPage(page, pageItems, searchWord, sort, idStaticTextType, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || !list.Any())
                    return NoContent();

                var ret = new ResultPageModel<StaticTextListItemModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<StaticTextListItemModel>()
                };
                foreach (var staticText in list.Page)
                    ret.Data.Add(new StaticTextListItemModel(staticText));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um text
        /// </summary>
        /// <param name="id">Id do texto desejada</param>
        /// <returns>StaticText</returns>
        /// <response code="200">Retorna o texto, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<StaticTextModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var staticText = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (staticText == null || staticText.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<StaticTextModel>() { Data = new StaticTextModel(staticText) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um texto
        /// </summary>
        /// <param name="staticText">StaticText</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] StaticTextModel staticText)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if(staticText != null)
            {
                if (repo.Update(staticText.GetEntity(), idAdminUser, out string error))
                    return Ok(new JsonModel() { Status = "ok", Message = "Texto atualizado com sucesso!" });

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto nulo" });

        }

        /// <summary>
        /// Cria um Texto
        /// </summary>
        /// <param name="staticText">StaticText</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] StaticTextModel staticText)
        {
            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var model = new JsonModel();

            var s = staticText.GetEntity();
            if(idOperation.HasValue)
                s.IdOperation = idOperation.Value;
            if (repo.Create(s, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Texto criado com sucesso!", Id = s.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um Texto
        /// </summary>
        /// <param name="id">Id do texto a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Texto apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}