using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IContactRepository repo;
        private IAddressRepository addrRepo;

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
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListAddress([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ContactModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<ContactModel>();
                foreach (var contact in list.Page)
                    ret.Data.Add(new ContactModel(contact));

                return Ok(ret);
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Retorna o contato conforme o ID
        /// </summary>
        /// <param name="id">Id do contato desejado</param>
        /// <returns>Contato</returns>
        /// <response code="201">Retorna o contato, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetContact(int id)
        {
            var contact = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (contact == null || contact.Id == 0)
                    return NoContent();
                return Ok(new { data = new ContactModel(contact) });
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Atualiza um contato
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] ContactModel contact)
        {
            var model = new JsonModel();
            var cont = contact.GetEntity();
            string error = null;

            if (contact.Address != null)
            {
                var addr = contact.Address.GetEntity();
                if (addrRepo.Update(addr, out error))
                    cont.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
            {
                model.Status = "error";
                model.Message = error;
            }
            else
            {
                if (repo.Update(cont, out error))
                {
                    model.Status = "ok";
                    model.Message = "Contato atualizado com sucesso!";
                }
                else
                {
                    model.Status = "error";
                    model.Message = error;
                }
            }
            return new JsonResult(model);
        }

        /// <summary>
        /// Cria um novo contato
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put([FromBody] ContactModel contact)
        {
            var model = new JsonModel();
            var cont = contact.GetEntity();
            string error = null;

            if(contact.Address != null)
            {
                var addr = contact.Address.GetEntity();
                if (addrRepo.Create(addr, out error))
                    cont.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
            {
                model.Status = "error";
                model.Message = error;
            }
            else
            {
                if (repo.Create(cont, out error))
                {
                    model.Status = "ok";
                    model.Message = "Contato criado com sucesso!";
                    model.Data = new { id = cont.Id };
                }
                else
                {
                    model.Status = "error";
                    model.Message = error;
                }
            }
            return new JsonResult(model);
        }

        /// <summary>
        /// Apaga um contato
        /// </summary>
        /// <param name="id">Id do contato a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();
            if (repo.Delete(id, out string error))
            {
                model.Status = "ok";
                model.Message = "Contato apagado com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }
    }
}