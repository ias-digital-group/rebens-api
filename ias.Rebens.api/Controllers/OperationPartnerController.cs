using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OperationPartnerController : BaseApiController
    {
        private IOperationPartnerRepository repo;
        private ICustomerRepository customerRepo;
        private IOperationRepository operationRepo;
        private IStaticTextRepository staticTextRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="operationPartnerRepository">Injeção de dependencia do repositório de parceiro</param>
        /// <param name="staticTextRepository">Injeção de dependencia do repositório de textos</param>
        /// <param name="operationRepository">Injeção de dependencia do repositório de operação</param>
        /// <param name="customerRepository">Injeção de dependencia do repositório de clientes</param>
        public OperationPartnerController(IOperationPartnerRepository operationPartnerRepository, IStaticTextRepository staticTextRepository,
                                            IOperationRepository operationRepository, ICustomerRepository customerRepository)
        {
            this.repo = operationPartnerRepository;
            this.staticTextRepo = staticTextRepository;
            this.operationRepo = operationRepository;
            this.customerRepo = customerRepository;
        }

        /// <summary>
        /// Retorna o parceiro da operação conforme o ID
        /// </summary>
        /// <param name="id">Id do parceiro desejada</param>
        /// <returns>Parceiro da Operação</returns>
        /// <response code="200">Retorna o parceiro, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher,promoter")]
        [ProducesResponseType(typeof(JsonDataModel<OperationPartnerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var partner = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (partner == null || partner.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<OperationPartnerModel>() { Data = new OperationPartnerModel(partner) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todas os parceiros de uma operação com paginação
        /// </summary>
        /// <param name="idOperation">id da operação, não obrigatório</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <returns>Lista com os parceiros da operação encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator")]
        [ProducesResponseType(typeof(ResultPageModel<OperationPartnerListItem>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]bool? active = null, [FromQuery]int? idOperation = null)
        {
            string error = null;
            if (CheckRole("administrator"))
                idOperation = GetOperationId(out error);

            if (string.IsNullOrEmpty(error))
            {
                var list = repo.ListPage(page, pageItems, searchWord, sort, out error, active, idOperation);

                if (string.IsNullOrEmpty(error))
                {
                    if (list == null || list.TotalItems == 0)
                        return NoContent();

                    var ret = new ResultPageModel<OperationPartnerListItem>
                    {
                        CurrentPage = list.CurrentPage,
                        HasNextPage = list.HasNextPage,
                        HasPreviousPage = list.HasPreviousPage,
                        ItemsPerPage = list.ItemsPerPage,
                        TotalItems = list.TotalItems,
                        TotalPages = list.TotalPages,
                        Data = new List<OperationPartnerListItem>()
                    };
                    foreach (var operation in list.Page)
                        ret.Data.Add(new OperationPartnerListItem(operation));

                    return Ok(ret);
                }
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um parceiro da operação
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]OperationPartnerModel partner)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var op = partner.GetEntity();
            if (repo.Update(op, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Parceiro da operação atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um parceiro de uma  operação
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id e o Código do parceiro criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] OperationPartnerModel partner)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var op = partner.GetEntity();
            if (repo.Create(op, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Parceiro criado com sucesso!", Id = op.Id });
            
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um parceiro de uma operação
        /// </summary>
        /// <param name="id">Id do parceiro a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Parceiro apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa/Inativa um parceiro de uma operação
        /// </summary>
        /// <param name="id">id do parceiro</param>
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

        #region OperationPartnerCustomer
        /// <summary>
        /// Lista os clientes de um parceiro da operação 
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="status">Status do cliente (Novo = 1, Aprovado = 2, Reprovado = 3, Cadastrado = 4), (default = null)</param>
        /// <param name="idOperation">id da operação</param>
        /// <param name="idOperationPartner">id do parceiro da operação</param>
        /// <returns>lista dos banners da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListCustomers"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,partnerAdministrator,partnerApprover,administrator")]
        [ProducesResponseType(typeof(ResultPageModel<OperationPartnerCustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCustomers([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? status = null, [FromQuery]int? idOperation = null, [FromQuery]int? idOperationPartner = null)
        {
            if (!idOperation.HasValue || idOperation.Value == 0)
            {
                if (CheckRoles(new string[] { "administrator", "partnerAdministrator" }))
                {
                    idOperation = GetOperationId(out string errorId);
                    if(errorId != null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
                }
            }
            if(!idOperationPartner.HasValue || idOperationPartner.Value == 0)
            {
                idOperationPartner = null;
                if (CheckRole("partnerAdministrator"))
                {
                    idOperationPartner = GetOperationPartnerId(out string errorId);
                    if (errorId != null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
                }
            }

            var list = customerRepo.ListPage(page: page, pageItems: pageItems, word: searchWord, sort: sort, error: out string error,
                                                idOperation: idOperation, idOperationPartner: idOperationPartner);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<OperationPartnerCustomerModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<OperationPartnerCustomerModel>()
                };
                foreach (var customer in list.Page)
                    ret.Data.Add(new OperationPartnerCustomerModel(customer));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um cliente de um parceiro da operação
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id e o Código do cliente criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SaveCustomer"), AllowAnonymous]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveCustomer([FromBody] OperationPartnerCustomerModel customer)
        {
            if(customer == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto nulo!" });

            var cust = customer.GetEntity();
            int idAdminUser = 0;
            if (CheckRole("partnerAdministrator"))
            {
                cust.IdOperationPartner = GetOperationPartnerId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

                idAdminUser = GetAdminUserId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            if (customerRepo.Create(cust, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = cust.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza o status de cliente do parceiro da operação
        /// </summary>
        /// <param name="idCustomer">Id do cliente</param>
        /// <param name="status">Satus (Aprovado = 2, Reprovado = 3)</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut("UpdateCustomerStatus"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,partnerAdministrator,partnerApprover,administrator")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UpdateCustomerStatus([FromQuery]int idCustomer, [FromQuery]int status)
        {
            if (status != 2 && status != 3)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "O status deve ser 2, para cadastro aprovado, ou 3, para cadastro reprovado!" });

            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId }); ;
           

            if(customerRepo.ChangeComplementaryStatus(idCustomer, (Enums.CustomerComplementaryStatus)status, out string error))
            {
                var customer = customerRepo.Read(idCustomer, out _);
                var operation = operationRepo.Read(customer.IdOperation, out _);

                string fromEmail = operationRepo.GetConfigurationOption(operation.Id, "contact-email", out _);
                if (string.IsNullOrEmpty(fromEmail) || !Helper.EmailHelper.IsValidEmail(fromEmail)) fromEmail = "contato@rebens.com.br";
                if (status == (int)Enums.CustomerComplementaryStatus.approved)
                    Helper.EmailHelper.SendCustomerValidation(staticTextRepo, operation, customer, fromEmail, out error);
                else if (status == (int)Enums.CustomerComplementaryStatus.reproved)
                {
                    string body = $"<p>Olá {customer.Name},</p><br /><p>Infelizmente o seu cadastro para acesso ao clube não foi aprovado.</p><br /><p>Grato</p>";
                    Helper.EmailHelper.SendDefaultEmail(staticTextRepo, customer.Email, customer.Name, operation.Id, $"{operation.Title} - Validação de Cadastro", body, fromEmail, operation.Title, out error);
                }
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Status do cliente atualizado com sucesso!" });
            }
            

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion OperationPartnerCustomer
    }
}