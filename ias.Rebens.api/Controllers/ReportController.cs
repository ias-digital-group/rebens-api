using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseApiController
    {
        private IReportRepository repo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportRepository"></param>
        public ReportController(IReportRepository reportRepository)
        {
            this.repo = reportRepository;
        }

        /// <summary>
        /// Método que retorna as informações necessárias para montar o dashboard do admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoadDashboard"), Authorize("Bearer", Roles = "administrator,master,administratorRebens,publisher,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<Dashboard>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult LoadDashboard()
        {
            int? idOperation = null;
            var principal = HttpContext.User;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var ret = new JsonDataModel<Dashboard>
            {
                Data = repo.LoadDashboard(out string error, null, null, idOperation)
            };
            if (string.IsNullOrEmpty(error))
                return Ok(ret);

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Relatório de clientes cadastrados no sistema
        /// </summary>
        /// <param name="idOperation">id da operação (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Name, Id, Email, Birthday), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idPartner">id do parceiro (default=null)</param>
        /// <param name="status">status do cliente (default=null)</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        /// <returns></returns>
        [HttpGet("ListCustomers"), Authorize("Bearer", Roles = "administrator,master,administratorRebens")]
        [ProducesResponseType(typeof(ResultPageModel<CustomerListItem>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCustomers([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null, [FromQuery]int? idPartner = null, [FromQuery]int? status = null)
        {
            if (CheckRole("administrator"))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            if (idOperation == 0) idOperation = null;
            if (idPartner == 0) idPartner = null;
            if (status == 0) status = null;

            var list = repo.ListCustomerPage(page, pageItems, searchWord, sort, out string error, idOperation, idPartner, status);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CustomerListItem>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CustomerListItem>()
                };
                foreach (var customer in list.Page)
                    ret.Data.Add(new CustomerListItem(customer));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Relatório de clientes cadastrados no sistema
        /// </summary>
        /// <param name="idOperation">id da operação (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Name, Id, Email, Birthday), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="startDate">Filtro data inicial</param>
        /// <param name="endDate">Filtro data final</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        /// <returns></returns>
        [HttpGet("ListBenefitUse"), Authorize("Bearer", Roles = "administrator,master,administratorRebens")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitListUseItem>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefitUse([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", 
            [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null, [FromQuery]DateTime? startDate = null, [FromQuery]DateTime? endDate = null)
        {
            if (CheckRole("administrator"))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListBenefitUsePage(page, pageItems, searchWord, sort, out string error, idOperation, startDate, endDate);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitListUseItem>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BenefitListUseItem>()
                };
                foreach (var item in list.Page)
                    ret.Data.Add(new BenefitListUseItem(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }


    }
}