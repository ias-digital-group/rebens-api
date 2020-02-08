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
    public class PromoterController : ControllerBase
    {
        private ICustomerPromoterRepository repo;
        private IOperationRepository operationRepo;
        private ICustomerRepository customerRepo;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="customerPromoterRepository"></param>
        public PromoterController(ICustomerPromoterRepository customerPromoterRepository, IOperationRepository operationRepository,
                                    ICustomerRepository customerRepository)
        {
            this.repo = customerPromoterRepository;
            this.operationRepo = operationRepository;
            this.customerRepo = customerRepository;
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
                                    [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "")
        {
            var principal = HttpContext.User;
            if (principal.IsInRole("promoter"))
            {
                if (principal?.Claims != null)
                {
                    var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                    if (userId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });
                    if (int.TryParse(userId.Value, out int tmpId))
                        idPromoter = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });
            }
            else if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation, idPromoter);

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
        public IActionResult Post([FromBody]CustomerModel customer)
        {
            int idPromoter = 0, idOperation = 0;
            if (customer == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto customer não pode ser nulo!" });

            var principal = HttpContext.User;
            if (principal.IsInRole("promoter"))
            {
                if (principal?.Claims != null)
                {
                    var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                    if (userId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });
                    if (!int.TryParse(userId.Value, out idPromoter))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });

                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (!int.TryParse(operationId.Value, out idOperation))
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Promotor não encontrada!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Você não tem permissão para realizar esse processo!" });

            var operation = operationRepo.Read(idOperation, out _);
            if (operation != null)
            {
                var cust = customer.GetEntity();
                cust.IdOperation = idOperation;
                cust.Status = (int)Enums.CustomerStatus.Validation;
                cust.CustomerType = (int)Enums.CustomerType.Customer;

                string password = Helper.SecurityHelper.CreatePassword();
                cust.SetPassword(password);
                if (repo.Create(cust, idPromoter, out string error))
                {
                    var sendingBlue = new Integration.SendinBlueHelper();
                    var body = $"<p>Seu cadastro foi realizado com sucesso!</p><p>Segue a sua senha temporária, sugerimos que você troque essa senha imediatamente:<br />Senha:<b>{password}</b></p>";
                    var listDestinataries = new Dictionary<string, string>() { { cust.Email, cust.Name } };
                    var result = sendingBlue.Send(listDestinataries, "contato@rebens.com.br", operation.Title, "Cadatro realizado com sucesso", body);

                    try
                    {
                        if (sendingBlue.CreateContact(cust, null, operation, out int blueId, out string error1))
                            customerRepo.SaveSendingblueId(cust.Id, blueId, out error1);
                    }
                    catch { }

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = customer.Id });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });
        }
    }
}