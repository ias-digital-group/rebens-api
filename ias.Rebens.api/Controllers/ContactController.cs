using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                if (contact != null || contact.Id == 0)
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
    }
}