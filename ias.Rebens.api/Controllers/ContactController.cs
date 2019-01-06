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

        public ContactController(IContactRepository contactRepository)
        {
            this.repo = contactRepository;
        }

        /// <summary>
        /// Retorna um contato
        /// </summary>
        /// <param name="id">id do contato</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public JsonResult GetContact(int id)
        {
            var contact = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Data = new ContactModel(contact);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        /// <summary>
        /// Atualiza um contato
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Post([FromBody] ContactModel contact)
        {
            var model = new JsonModel();
            var cont = contact.GetEntity();
            string error = null;

            if (contact.Address != null)
            {
                var addrRepo = ServiceLocator<IAddressRepository>.Create();
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
        public JsonResult Put([FromBody] ContactModel contact)
        {
            var model = new JsonModel();
            var cont = contact.GetEntity();
            string error = null;

            if(contact.Address != null)
            {
                var addrRepo = ServiceLocator<IAddressRepository>.Create();
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