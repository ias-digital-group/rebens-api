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
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
    [ApiController]
    public class ZanoxIncentiveController : BaseApiController
    {
        private IZanoxIncentiveRepository repo;

        public ZanoxIncentiveController(IZanoxIncentiveRepository zanoxIncentive) 
        {
            repo = zanoxIncentive;
        }

        /// <summary>
        /// Lista os Incentivos da Zanox
        /// </summary>
        /// <param name="page">pagina (default = 0)</param>
        /// <param name="pageItems">itens por página (default = 30)</param>
        /// <param name="searchWord">palavra à ser buscada</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<ZanoxIncentiveModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 30, [FromQuery] string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, null, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ZanoxIncentiveModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ZanoxIncentiveModel>()
                };
                foreach (var item in list.Page)
                    ret.Data.Add(new ZanoxIncentiveModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

    }
}
