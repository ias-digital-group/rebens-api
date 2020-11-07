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
    public class CustomerReferalController : BaseApiController
    {
        private ICustomerRepository repo;
        private IOperationRepository operationRepo;
        private IStaticTextRepository staticTextRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerReferalRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="staticTextRepository"></param>
        public CustomerReferalController(ICustomerRepository customerRepository, IOperationRepository operationRepository, IStaticTextRepository staticTextRepository)
        {
            this.repo = customerRepository;
            this.operationRepo = operationRepository;
            this.staticTextRepo = staticTextRepository;
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
        [HttpGet, Authorize("Bearer", Roles = "master,administrator,customer,administratorRebens")]
        [ProducesResponseType(typeof(ResultPageModel<CustomerReferalModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            ResultPage<Customer> list;
            string error;
            int idCustomer = 0;
            if (CheckRole("customer"))
            {
                idCustomer = GetCustomerId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
                list = repo.ListReferalByCustomer(idCustomer, page, pageItems, searchWord, sort, out error);
            }
            else
            {
                int? idOperation = null;
                if (CheckRole("administrator"))
                {
                    idOperation = GetOperationId(out string errorId);
                    if (errorId != null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
                }
                list = repo.ListReferalPage(page, pageItems, searchWord, sort, idOperation, out error);
            }
                

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CustomerReferalModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CustomerReferalModel>()
                };
                
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
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,administrator,customer,administratorRebens")]
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
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            int idOperation = GetOperationId(out errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var operation = operationRepo.Read(idOperation, out string error);
            var customer = repo.Read(idCustomer, out error);

            if(repo.CheckReferalLimit(operation.Id, customer.Id, out int limit, out error))
            {
                var referal = customerReferal.GetEntity();
                referal.ComplementaryStatus = (int)Enums.CustomerComplementaryStatus.pending;
                referal.Status = (int)Enums.CustomerStatus.PreSignup;
                referal.IdCustomerReferer = idCustomer;
                referal.IdOperation = idOperation;
                referal.Created = referal.Modified = DateTime.Now;
                referal.CustomerType = (int)Enums.CustomerType.Referal;
                if (repo.Create(referal, 0, out error))
                {
                    string fromEmail = operationRepo.GetConfigurationOption(operation.Id, "contact-email", out _);
                    string color = operationRepo.GetConfigurationOption(operation.Id, "color", out _);
                    if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";

                    Helper.EmailHelper.SendCustomerReferal(staticTextRepo, operation, referal, fromEmail, color, out error);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Indicação criada com sucesso!", Id = referal.Id });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = $"Você já realizou as {limit} indicações permitidas para sua conta." });
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
            if (repo.DeleteReferal(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Indicação apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

    }
}