using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        /// Retorna uma lista de endereços conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os endereços encontradas</returns>
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

                var ret = new ResultPageModel<AddressModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<AddressModel>();
                foreach (var addr in list.Page)
                    ret.Data.Add(new AddressModel(addr));

                return Ok(ret);
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
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
                if (addr == null || addr.Id == 0)
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
        public IActionResult Put([FromBody] AddressModel address)
        {
            var model = new JsonModel();

            if (repo.Update(address.GetEntity(), out string error))
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
        /// <param name="address"></param>
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

        /// <summary>
        /// Apaga um endereço
        /// </summary>
        /// <param name="id">Id do endereço a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();
            if (repo.Delete(id, out string error))
            {
                model.Status = "ok";
                model.Message = "Endereço apagado com sucesso!";
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