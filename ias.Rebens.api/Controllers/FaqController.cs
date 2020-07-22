using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Faq Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/Faq"), Authorize("Bearer", Roles = "master,publisher,administrator,administratorRebens,publisherRebens")]
    [ApiController]
    public class FaqController : BaseApiController
    {
        private IFaqRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="faqRepository"></param>
        public FaqController(IFaqRepository faqRepository)
        {
            this.repo = faqRepository;
        }

        /// <summary>
        /// Lista as perguntas de faq conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Question, Answer, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da Operação</param>
        /// <returns>Lista com as faqs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<FaqModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Question ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null)
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

                var ret = new ResultPageModel<FaqModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<FaqModel>()
                };
                
                foreach (var faq in list.Page)
                    ret.Data.Add(new FaqModel(faq));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma pergunta
        /// </summary>
        /// <param name="id">Id da pergunta desejada</param>
        /// <returns>FAQ</returns>
        /// <response code="200">Retorna a faq, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<FaqModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var faq = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (faq == null || faq.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<FaqModel>() { Data = new FaqModel(faq) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma pergunta
        /// </summary>
        /// <param name="faq">FAQ</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] FaqModel faq)
        {
            int idAdminUser = GetAdminUserId(out string errorIdUser);
            if (errorIdUser != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorIdUser });

            if (repo.Update(faq.GetEntity(), idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Pergunta atualizado com sucesso!" });
            
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma pergunta
        /// </summary>
        /// <param name="faq"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] FaqModel faq)
        {
            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            int idAdminUser = GetAdminUserId(out string errorIdUser);
            if (errorIdUser != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorIdUser });

            var f = faq.GetEntity();
            if (idOperation.HasValue)
                f.IdOperation = idOperation.Value;
            if (repo.Create(f, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Pergunta criado com sucesso!", Id = f.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma pergunta
        /// </summary>
        /// <param name="id">Id da pergunta a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorIdUser);
            if (errorIdUser != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorIdUser });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Pergunta apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
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