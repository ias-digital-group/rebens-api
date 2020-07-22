using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Bank Account Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/BankAccount"), Authorize("Bearer", Roles = "customer")]
    [ApiController]
    public class BankAccountController : BaseApiController
    {
        private IBankAccountRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bankAccountRepository"></param>
        public BankAccountController(IBankAccountRepository bankAccountRepository)
        {
            this.repo = bankAccountRepository;
        }

        /// <summary>
        /// Lista todos as contas de um cliente com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Branch, Account, Type), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as contas encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<BankAccountModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var list = repo.ListPage(idCustomer, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BankAccountModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BankAccountModel>()
                };
                foreach (var part in list.Page)
                    ret.Data.Add(new BankAccountModel(part));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a conta conforme o ID
        /// </summary>
        /// <param name="id">Id da conta</param>
        /// <returns>Conta</returns>
        /// <response code="200">Retorna a conta, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<BankAccountModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var account = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (account == null || account.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<BankAccountModel>() { Data = new BankAccountModel(account) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma conta
        /// </summary>
        /// <param name="account"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]BankAccountModel account)
        {
            var part = account.GetEntity();
            if (repo.Update(part, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Conta atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma conta
        /// </summary>
        /// <param name="account"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da conta criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]BankAccountModel account)
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var part = account.GetEntity();
            part.IdCustomer = idCustomer;
            if (repo.Create(part, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Conta criada com sucesso!", Id = part.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma conta
        /// </summary>
        /// <param name="id">Id da conta a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Conta apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os Bancos
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a lista, ou algum erro, caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [AllowAnonymous]
        [HttpGet("ListBanks")]
        [ProducesResponseType(typeof(JsonDataModel<List<BankModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBanks()
        {
            var list = repo.ListBanks(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BankModel>>();
                ret.Data = new List<BankModel>();
                list.ForEach(item => { ret.Data.Add(new BankModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}