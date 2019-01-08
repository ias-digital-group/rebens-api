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
        private IAddressRepository repo;
        
        public AddressController(IAddressRepository addressRepository)
        {
            this.repo = addressRepository;
        }

        /// <summary>
        /// Retorna o endereço conforme o ID
        /// </summary>
        /// <param name="id">Id do endereço desejada</param>
        /// <returns>Endereço</returns>
        /// <response code="201">Retorna a categoria, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var addr = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (addr != null || addr.Id == 0)
                    return NoContent();

                return Ok(new { data = new AddressModel(addr) });
            }


            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Atualiza um endereço
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody] AddressModel addr)
        {
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

            return Ok(model);
        }

        /// <summary>
        /// Cria um endereço
        /// </summary>
        /// <param name="address'"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id da categoria criada</returns>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult Post([FromBody] AddressModel address)
        {
            var model = new JsonModel();
            var addr = address.GetEntity();

            if (repo.Create(addr, out string error))
            {
                model.Status = "ok";
                model.Message = "Endereço criado com sucesso!";
                model.Data = new { id = addr.Id };
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