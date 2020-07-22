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
    /// Promoter Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens,promoter")]
    [ApiController]
    public class PromoterController : BaseApiController
    {
        private ICustomerRepository repo;
        private IOperationRepository operationRepo;
        private IStaticTextRepository staticTextRepo;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="customerRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="staticTextRepository"></param>
        public PromoterController(ICustomerRepository customerRepository, IOperationRepository operationRepository,
                                    IStaticTextRepository staticTextRepository)
        {
            this.repo = customerRepository;
            this.operationRepo = operationRepository;
            this.staticTextRepo = staticTextRepository;
        }

        /// <summary>
        /// List
        /// </summary>
        /// <param name="idOperation">Operation ID</param>
        /// <param name="idPromoter">Promoter ID</param>
        /// <param name="page">current page</param>
        /// <param name="pageItems">items per page</param>
        /// <param name="sort">sort (name, email, operation, status, cpf) (' ASC', ' DESC')</param>
        /// <param name="searchWord"></param>
        /// <returns>List of customers</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int? idOperation = null, [FromQuery]int? idPromoter = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, 
                                    [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? status = null)
        {
            if (CheckRole("promoter"))
            {
                idPromoter = GetAdminUserId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }
            else if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page: page, pageItems: pageItems, word: searchWord, sort: sort, error: out string error, 
                                        idOperation: idOperation, status: status, idPromoter: idPromoter);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CustomerModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CustomerModel>()
                };
                foreach (var customer in list.Page)
                {
                    var tmp = new CustomerModel(customer);
                    tmp.Cpf = tmp.Cpf.Replace(tmp.Cpf.Substring(2, 5), "*****");
                    ret.Data.Add(tmp);
                }

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

                var tmp = new CustomerModel(customer);
                tmp.Cpf = tmp.Cpf.Replace(tmp.Cpf.Substring(2, 5), "*****");
                return Ok(new JsonDataModel<CustomerModel>() { Data = tmp });
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
        public IActionResult Post([FromBody]SignUpModel customer)
        {
            if (customer == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto customer não pode ser nulo!" });
            int idPromoter;
            int idOperation;
            if (CheckRole("promoter"))
            {
                idPromoter = GetAdminUserId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

                idOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Você não tem permissão para realizar esse processo!" });

            var operation = operationRepo.Read(idOperation, out _);
            if (operation != null)
            {
                var cust = new Customer()
                {
                    Email = customer.Email,
                    Cpf = customer.Cpf,
                    Name = customer.Name + " " + customer.Surname,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Status = (int)Enums.CustomerStatus.Validation,
                    CustomerType = (int)Enums.CustomerType.Promoter,
                    Code = Helper.SecurityHelper.HMACSHA1(customer.Email, customer.Email + "|" + customer.Cpf),
                    IdOperation = operation.Id,
                    IdPromoter = idPromoter
                };

                if (repo.Create(cust, out string error))
                {
                    string fromEmail = operationRepo.GetConfigurationOption(operation.Id, "contact-email", out _);
                    if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                    Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, cust, fromEmail, out error);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Enviamos um e-mail para ativação do cadastro.", Id = cust.Id });
                }
                
                if(error == "cpf-registered" || error == "email-registered")
                    return StatusCode(400, new JsonModel() { Status = error });
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Reenvia o email de validação para o cliente
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id do cliente criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("ResendValidation/{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ResendValidation(int id)
        {
            if (id <= 0)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "O id do cliente é obrigatório!" });

            int idPromoter;
            int idOperation;
            if (CheckRole("promoter"))
            {
                idPromoter = GetAdminUserId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

                idOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Você não tem permissão para realizar esse processo!" });

            var operation = operationRepo.Read(idOperation, out _);
            if (operation != null)
            {
                var cust = repo.Read(id, out string error);
                if (string.IsNullOrEmpty(error))
                {
                    string fromEmail = operationRepo.GetConfigurationOption(operation.Id, "contact-email", out _);
                    if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                    Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, cust, fromEmail, out error);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "E-mail reenviado com sucesso." });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }

        /// <summary>
        /// Cria um Cliente
        /// </summary>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id do cliente criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Report"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<PromoterReportModel>), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Report([FromQuery]int? idOperation = null, [FromQuery]int page = 0, [FromQuery]int pageItems = 30,
                                    [FromQuery]string searchWord = "")
        {
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.Report(page, pageItems, searchWord, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {

                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<PromoterReportModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = list.Page.ToList()
                };

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}