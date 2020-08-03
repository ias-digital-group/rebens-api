using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Customer Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,administratorRebens")]
    [ApiController]
    public class CustomerController : BaseApiController
    {
        private ICustomerRepository repo;
        private IAddressRepository addrRepo;
        private IOperationRepository operationRepo;
        private IStaticTextRepository staticTextRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerRepository"></param>
        public CustomerController(ICustomerRepository customerRepository,
                                    IAddressRepository addressRepository,
                                    IOperationRepository operationRepository,
                                    IStaticTextRepository staticTextRepository)
        {
            this.repo = customerRepository;
            this.addrRepo = addressRepository;
            this.operationRepo = operationRepository;
            this.staticTextRepo = staticTextRepository;
        }

        /// <summary>
        /// Retorna uma lista de endereços conforme os parametros
        /// </summary>
        /// <param name="idOperation">id da operação (default=null)</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Name, Id, Email, Birthday), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int? idOperation = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, 
                                    [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperationPartner = null,
                                    [FromQuery]int? status = null, [FromQuery]bool? active = null)
        {
            if (CheckRole("administrator"))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation, idOperationPartner, status, active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CustomerModel>() { 
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CustomerModel>()
                };
                foreach (var customer in list.Page)
                    ret.Data.Add(new CustomerModel(customer));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }


        /// <summary>
        /// Retorna o endereço conforme o ID
        /// </summary>
        /// <param name="id">Id do cliente desejada</param>
        /// <returns>Endereço</returns>
        /// <response code="200">Retorna a categoria, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var customer = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {

                if (customer == null || customer.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<CustomerModel>() { Data = new CustomerModel(customer) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um Cliente
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id do cliente criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CustomerModel customer)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var custo = customer.GetEntity();
            if (customer.IdOperationPartner.HasValue && customer.IdOperationPartner.Value > 0)
            {
                custo.IdOperationPartner = customer.IdOperationPartner.Value;
                custo.ComplementaryStatus = (int)Enums.CustomerComplementaryStatus.approved;
            }
            custo.Code = Helper.SecurityHelper.HMACSHA1(custo.Email, custo.Email + "|" + custo.Cpf);

            if (repo.Create(custo, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = custo.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um Cliente
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CustomerModel customer)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            string error = null;

            var custo = customer.GetEntity();
            var addr = customer.GetAddress();
            if (addr != null)
            {
                if (addr.Id > 0)
                    addrRepo.Update(addr, idAdminUser, out _);
                else
                    addrRepo.Create(addr, idAdminUser, out _);

                custo.IdAddress = addr.Id;
            }

            if (repo.Update(custo, idAdminUser, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Cliente atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um cliente
        /// </summary>
        /// <param name="id">Id do cliente a ser apagada</param>
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
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Reenvia o email de validação do cliente
        /// </summary>
        /// <param name="id">id do usuário</param>
        /// <returns></returns>
        /// <response code="200">Se o e-mail for enviado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ResendValidation/{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ResendValidation(int id)
        {
            var customer = repo.Read(id, out string error);

            if (customer != null)
            {
                var operation = operationRepo.Read(customer.IdOperation, out error);
                string fromEmail = operationRepo.GetConfigurationOption(customer.IdOperation, "contact-email", out _);
                if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, customer, fromEmail, out error);

                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "E-mail de validação reenviado com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "Ocorreu um erro ao tentar reenviar a validação!" });
        }

        /// <summary>
        /// Ativa/Inativa um cliente
        /// </summary>
        /// <param name="id">id do cliente</param>
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