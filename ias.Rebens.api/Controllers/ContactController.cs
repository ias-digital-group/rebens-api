using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Collections.Generic;
using System.Linq;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Contact Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,publisher,administrator,administratorRebens,publisherRebens")]
    [ApiController]
    public class ContactController : BaseApiController
    {
        private IContactRepository repo;
        private IAddressRepository addrRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contactRepository"></param>
        /// <param name="addressRepository"></param>
        public ContactController(IContactRepository contactRepository, IAddressRepository addressRepository)
        {
            this.repo = contactRepository;
            this.addrRepo = addressRepository;
        }

        /// <summary>
        /// Retorna uma lista de contatos conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Email, JobTitle), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">Status</param>
        /// <param name="type">Tipo de contato</param>
        /// <param name="idItem">Id do vínculo do contato</param>
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<ContactModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", 
                                    [FromQuery]string searchWord = "", [FromQuery]bool? active = null, [FromQuery]int? type = null, [FromQuery]int? idItem = null)
        {
            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, type, idItem, active, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ContactModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ContactModel>()
                };
                foreach (var contact in list.Page)
                    ret.Data.Add(new ContactModel(contact));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o contato conforme o ID
        /// </summary>
        /// <param name="id">Id do contato desejado</param>
        /// <returns>Contato</returns>
        /// <response code="200">Retorna o contato, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>b
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<ContactModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var contact = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (contact == null || contact.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<ContactModel>() { Data = new ContactModel(contact) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um contato
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] ContactModel contact)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var cont = contact.GetEntity();
            string error = null;

            if (contact.Address != null)
            {
                var addr = contact.Address.GetEntity();
                if (addrRepo.Update(addr, idAdminUser, out error))
                    cont.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            if (repo.Update(cont, idAdminUser, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Contato atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um novo contato
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] ContactModel contact)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var cont = contact.GetEntity();
            string error = null;

            if(contact.Address != null)
            {
                var addr = contact.Address.GetEntity();
                if (addrRepo.Create(addr, idAdminUser, out error))
                    cont.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            
            if (repo.Create(cont, idAdminUser, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Contato criado com sucesso!", Id = cont.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um contato
        /// </summary>
        /// <param name="id">Id do contato a ser apagado</param>
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
                return Ok(new JsonModel() { Status = "ok", Message = "Contato apagado com sucesso!" });
            
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa/Inativa um contato
        /// </summary>
        /// <param name="id">id do contato</param>
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