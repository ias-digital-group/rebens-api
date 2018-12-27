using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        [HttpGet("{id}")]
        public JsonResult GetContact(int id)
        {
            var repo = ServiceLocator<IContactRepository>.Create();
            var contact = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = new ContactModel(contact);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult Post([FromBody] ContactModel contact)
        {
            var repo = ServiceLocator<IContactRepository>.Create();
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

        [HttpPut]
        public JsonResult Put([FromBody] ContactModel contact)
        {
            var repo = ServiceLocator<IContactRepository>.Create();
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
                    model.Extra = new { id = cont.Id };
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