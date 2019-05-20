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
    [Route("api/[controller]")]
    [ApiController]
    public class OperationPartnerController : ControllerBase
    {
        private IOperationPartnerRepository repo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="operationPartnerRepository">Injeção de dependencia do repositório de parceiro</param>
        public OperationPartnerController(IOperationPartnerRepository operationPartnerRepository)
        {
            this.repo = operationPartnerRepository;
        }

        /// <summary>
        /// Retorna o parceiro da operação conforme o ID
        /// </summary>
        /// <param name="id">Id do parceiro desejada</param>
        /// <returns>Parceiro da Operação</returns>
        /// <response code="200">Retorna o parceiro, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator")]
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
        /// <param name="idOperation">id da operação, obrigatório</param>
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
        [ProducesResponseType(typeof(ResultPageModel<OperationPartnerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int idOperation, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]bool? active = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, idOperation, out string error, active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<OperationPartnerModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<OperationPartnerModel>();
                foreach (var operation in list.Page)
                    ret.Data.Add(new OperationPartnerModel(operation));

                return Ok(ret);
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
            var op = partner.GetEntity();
            if (repo.Update(op, out string error))
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
            var op = partner.GetEntity();
            if (repo.Create(op, out string error))
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
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Parceiro apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        #region OperationPartnerCustomer
        /// <summary>
        /// Lista os clientes de um parceiro da operação 
        /// </summary>
        /// <param name="id">id do parceiro da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="status">Status do cliente (Novo = 1, Aprovado = 2, Reprovado = 3, Cadastrado = 4), (default = null)</param>
        /// <returns>lista dos banners da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Customers"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,partnerAdministrator,partnerApprover,administrator")]
        [ProducesResponseType(typeof(ResultPageModel<OperationPartnerCustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCustomers(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]int? status = null)
        {
            var list = repo.ListCustomers(id, page, pageItems, searchWord, sort, out string error, status);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<OperationPartnerCustomerModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<OperationPartnerCustomerModel>();
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
            var cust = customer.GetEntity();
            if (repo.CreateCustomer(cust, out string error))
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
            if(status != 2 && status != 3)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "O status deve ser 2, para cadastro aprovado, ou 3, para cadastro reprovado!" });

            if (repo.UpdateCustomerStatus(idCustomer, status, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Status do cliente atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion OperationPartnerCustomer
    }
}