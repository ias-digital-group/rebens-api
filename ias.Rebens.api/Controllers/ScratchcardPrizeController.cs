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
    /// Faq Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/ScratchcardPrize"), Authorize("Bearer", Roles = "master,publisher,administrator,administratorRebens,publisherRebens")]
    [ApiController]
    public class ScratchcardPrizeController :  BaseApiController
    {
        private IScratchcardPrizeRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scratchcardPrizeRepository"></param>
        public ScratchcardPrizeController(IScratchcardPrizeRepository scratchcardPrizeRepository)
        {
            this.repo = scratchcardPrizeRepository;
        }

        /// <summary>
        /// Retorna a lista de Raspadinhas premiadas
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da operação, não obrigatório</param>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardPrizeListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPrizes([FromQuery] int page = 0, [FromQuery] int pageItems = 30,
                [FromQuery] string searchWord = "", [FromQuery] int? idOperation = null, [FromQuery] int? idScratchcard = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, out string error, idOperation, idScratchcard);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardPrizeListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardPrizeListItemModel>()
                };
                foreach (var prize in list.Page)
                    ret.Data.Add(new ScratchcardPrizeListItemModel(prize));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um prêmio
        /// </summary>
        /// <param name="id">Id do prêmio desejado</param>
        /// <returns>Prêmio</returns>
        /// <response code="200">Retorna o prêmio, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("read/{id}")]
        [ProducesResponseType(typeof(JsonDataModel<ScratchcardPrizeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var prize = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (prize == null || prize.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<ScratchcardPrizeModel>() { Data = new ScratchcardPrizeModel(prize) {
                    ImagePath = $"{Request.Scheme}://{Request.Host}/files/scratchcard/"
                } });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um prêmio
        /// </summary>
        /// <param name="prize">Prêmio</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]ScratchcardPrizeModel prize)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (prize != null)
            {
                if (repo.Update(prize.GetEntity(), idAdminUser, out string error))
                    return Ok(new JsonModel() { Status = "ok", Message = "Prêmio atualizado com sucesso!" });

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "O objeto não pode ser nulo" });
        }

        /// <summary>
        /// Cria um prêmio
        /// </summary>
        /// <param name="prize">premio</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]ScratchcardPrizeModel prize)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (prize != null)
            {
                var p = prize.GetEntity();
                if (repo.Create(p, idAdminUser, out string error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Prêmio criado com sucesso!", Id = p.Id });

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "O objeto não pode ser nulo" });
        }

        /// <summary>
        /// Apaga um prêmio
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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Prêmio apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}