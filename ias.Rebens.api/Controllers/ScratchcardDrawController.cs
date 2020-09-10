using System.Collections.Generic;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScratchcardDrawController : BaseApiController
    {
        private IScratchcardDrawRepository repo;
        public ScratchcardDrawController(IScratchcardDrawRepository drawRepository)
        {
            repo = drawRepository;
        }

        /// <summary>
        /// Retorna a lista de Raspadinhas
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
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardDrawListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 30,
                [FromQuery] string searchWord = "", [FromQuery] int? idOperation = null, [FromQuery] int? idScratchcard = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, out string error, idOperation, idScratchcard);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardDrawListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardDrawListItemModel>()
                };
                foreach (var scratchcard in list.Page)
                    ret.Data.Add(new ScratchcardDrawListItemModel(scratchcard));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a lista de Raspadinhas Premiadas
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da operação, não obrigatório</param>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("PrizedListPage"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardDrawListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult PrizedListPage([FromQuery] int page = 0, [FromQuery] int pageItems = 30,
                [FromQuery] string searchWord = "", [FromQuery] int? idOperation = null, [FromQuery] int? idScratchcard = null)
        {
            var list = repo.ScratchedWithPrizeListPage(page, pageItems, searchWord, idOperation, idScratchcard, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardDrawListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardDrawListItemModel>()
                };
                foreach (var scratchcard in list.Page)
                    ret.Data.Add(new ScratchcardDrawListItemModel(scratchcard));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
