using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/CourseCollege"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class CourseCollegeController : ControllerBase
    {
        private ICourseCollegeRepository repo;
        private IAddressRepository addressRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseCollegeRepository"></param>
        /// <param name="addressRepository"></param>
        public CourseCollegeController(ICourseCollegeRepository courseCollegeRepository, IAddressRepository addressRepository)
        {
            this.repo = courseCollegeRepository;
            this.addressRepo = addressRepository;
        }

        /// <summary>
        /// Lista as faculdades conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da Operação</param>
        /// <returns>Lista com as faculdades encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CourseCollegeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null)
        {
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseCollegeModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CourseCollegeModel>()
                };

                foreach (var item in list.Page)
                    ret.Data.Add(new CourseCollegeModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma faculdade
        /// </summary>
        /// <param name="id">Id da faculdade</param>
        /// <returns>Modalidade</returns>
        /// <response code="200">Retorna a faculdade, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CourseCollegeModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var item = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (item == null || item.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CourseCollegeModel>() { Data = new CourseCollegeModel(item) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma faculdade
        /// </summary>
        /// <param name="college">Faculdade</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CourseCollegeModel college)
        {
            if (repo.Update(college.GetEntity(), out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Faculdade atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma modalidade de curso
        /// </summary>
        /// <param name="college"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do período criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CourseCollegeModel college)
        {
            int? idOperation = null;
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var p = college.GetEntity();

            if (college.Address != null)
            {
                var addr = college.Address.GetEntity();
                if (addressRepo.Create(addr, out string errorAddr))
                    p.IdAddress = addr.Id;
            }

            if (idOperation.HasValue)
                p.IdOperation = idOperation.Value;
            if (repo.Create(p, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Faculdade criada com sucesso!", Id = p.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma faculdade
        /// </summary>
        /// <param name="id">Id da faculdade a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Faculdade apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }


        /// <summary>
        /// Lista os endereço de uma faculdade
        /// </summary>
        /// <param name="id">id da faculdade</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Address")]
        [ProducesResponseType(typeof(JsonDataModel<List<AddressModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListAddress(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = addressRepo.ListByCourseCollege(id, page, pageItems, searchWord, sort, out string error);

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

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um endereço a uma faculdade
        /// </summary>
        /// <param name="model">{ idCourseCollege: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula uma faculdade com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddAddress")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress([FromBody]CourseCollegeAddressModel model)
        {
            if (repo.AddAddress(model.IdCourseCollege, model.IdAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um endereço de uma faculdade
        /// </summary>
        /// <param name="id">id da faculdade</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{Id}/Address/{idAddress}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            if (repo.DeleteAddress(id, idAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}