using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        /// <summary>
        /// Retorna uma categoria
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Atualiza a categoria
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Cria uma categoria
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
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