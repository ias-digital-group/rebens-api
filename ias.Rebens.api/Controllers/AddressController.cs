using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        [HttpGet("{id}")]
        public JsonResult GetCategory(int id)
        {
            var repo = ServiceLocator<IAddressRepository>.Create();
            var addr = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = new AddressModel(addr);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult Post([FromBody] AddressModel addr)
        {
            var repo = ServiceLocator<IAddressRepository>.Create();
            var model = new JsonModel();

            if (repo.Update(addr.GetEntity(), out string error))
            {
                model.Status = "ok";
                model.Message = "Endereço atualizado com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPut]
        public JsonResult Put([FromBody] AddressModel address)
        {
            var repo = ServiceLocator<IAddressRepository>.Create();
            var model = new JsonModel();

            var addr = address.GetEntity();
            if (repo.Create(addr, out string error))
            {
                model.Status = "ok";
                model.Message = "Endereço criado com sucesso!";
                model.Extra = new { id = addr.Id };
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }
    }
}