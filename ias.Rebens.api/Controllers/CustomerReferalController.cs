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
    [Route("api/CustomerReferal")]
    [ApiController]
    public class CustomerReferalController : ControllerBase
    {
        private ICustomerReferalRepository repo;
        private ICustomerRepository customerRepo;
        private IOperationRepository operationRepo;

        public CustomerReferalController(ICustomerReferalRepository customerReferalRepository, ICustomerRepository customerRepository, IOperationRepository operationRepository)
        {
            this.repo = customerReferalRepository;
            this.customerRepo = customerRepository;
            this.operationRepo = operationRepository;
        }

        /// <summary>
        /// Lista as Indicações conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Email, Status), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as Indicações encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "administrator, customer")]
        [ProducesResponseType(typeof(ResultPageModel<CustomerReferalModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            ResultPage<CustomerReferal> list;
            string error;
            int idCustomer = 0;
            var principal = HttpContext.User;
            if (principal != null && principal.IsInRole("customer"))
            {

                if (principal?.Claims != null)
                {
                    var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                    if (customerId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                    if (!int.TryParse(customerId.Value, out idCustomer))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                }
                list = repo.ListByCustomer(idCustomer, page, pageItems, searchWord, sort, out error);
            }
            else 
                list = repo.ListPage(page, pageItems, searchWord, sort, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CustomerReferalModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<CustomerReferalModel>();
                foreach (var cr in list.Page)
                    ret.Data.Add(new CustomerReferalModel(cr));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma Indicação
        /// </summary>
        /// <param name="id">Id do banner desejada</param>
        /// <returns>Banner</returns>
        /// <response code="200">Retorna o banner, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "administrator, customer")]
        [ProducesResponseType(typeof(JsonDataModel<CustomerReferalModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var referal = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (referal == null || referal.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CustomerReferalModel>() { Data = new CustomerReferalModel(referal) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma indicação
        /// </summary>
        /// <param name="customerReferal">indicação</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] CustomerReferalModel customerReferal)
        {
            var model = new JsonModel();

            if (repo.Update(customerReferal.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Indicação atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma Indicação
        /// </summary>
        /// <param name="customerReferal">Indicação</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da indicação criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] CustomerReferalModel customerReferal)
        {
            int idCustomer = 0;
            int idOperation = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (customerId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });
                if (!int.TryParse(customerId.Value, out idCustomer))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Cliente não encontrado!" });

                var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (operationId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                if (!int.TryParse(operationId.Value, out idOperation))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var operation = operationRepo.Read(idOperation, out string error);
            var customer = customerRepo.Read(idCustomer, out error);



            var model = new JsonModel();

            var referal = customerReferal.GetEntity();
            referal.IdStatus = (int)Enums.CustomerReferalStatus.pending;
            referal.IdCustomer = idCustomer;
            referal.Created = referal.Modified = DateTime.Now;
            if (repo.Create(referal, out error))
            {
                var sendingBlue = new Integration.SendinBlueHelper();
                var body = $"<p>Olá {referal.Name}<br /><br />Você foi convidado para participar do clube: {operation.Title}</p>";
                sendingBlue.Send(referal.Email, referal.Name, "contato@rebens.com.br", "Contato", "Indicação - " + operation.Title, body);

                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Indicação criada com sucesso!", Id = referal.Id });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma indicação
        /// </summary>
        /// <param name="id">Id da indicação a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();

            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Indicação apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

    }
}