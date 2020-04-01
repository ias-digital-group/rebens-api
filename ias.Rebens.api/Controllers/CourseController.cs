using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ias.Rebens.api.Models;
using System;
using System.Linq;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Course Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private ICourseRepository repo;
        private IAddressRepository addressRepo;
        private IStaticTextRepository staticTextRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="courseRepository">Injeção de dependencia do repositório de Curso</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de Endereço</param>
        /// <param name="staticTextRepository">Injeção de dependencia do repositório de Texto Estático</param>
        public CourseController(ICourseRepository courseRepository, IAddressRepository addressRepository, IStaticTextRepository staticTextRepository)
        {
            this.repo = courseRepository;
            this.addressRepo = addressRepository;
            this.staticTextRepo = staticTextRepository;

        }

        /// <summary>
        /// Lista todos os cursos com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">id da operação, não obrigatório (default=null)</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <returns>Lista com os cursos encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CourseModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "",
            [FromQuery]int? idOperation = null, [FromQuery]bool? active = null)
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

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation: idOperation, status: active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CourseModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CourseModel>()
                };
                foreach (var item in list.Page)
                    ret.Data.Add(new CourseModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "" });
        }

        /// <summary>
        /// Retorna o curso conforme o ID
        /// </summary>
        /// <param name="id">Id do curso</param>
        /// <returns>Curso</returns>
        /// <response code="200">Retorna o curso, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CourseModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var course = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (course == null || course.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CourseModel>() { Data = new CourseModel(course) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]CourseModel course)
        {
            var principal = HttpContext.User;
            var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
            if (customerId == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            if (!int.TryParse(customerId.Value, out int idAdmin))
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            var part = course.GetEntity();
            part.IdAdminUser = idAdmin;
            if (repo.Update(part, out string error))
            {
                if (!string.IsNullOrEmpty(course.Description))
                {
                    var detail = course.GetDescription();
                    if (part.IdDescription.HasValue)
                    {
                        detail.Id = part.IdDescription.Value;
                        staticTextRepo.Update(detail, out error);
                    }
                    else
                    {
                        staticTextRepo.Create(detail, out error);
                        part.IdDescription = detail.Id;
                        repo.Update(part, out error);
                    }
                }

                if (repo.RemovePeriods(part.Id, out error))
                {
                    if (course.PeriodIds != null && course.PeriodIds.Length > 0)
                    {
                        foreach (var periodId in course.PeriodIds)
                            repo.AddPeriod(part.Id, periodId, out error);
                    }
                }

                return Ok(new JsonModel() { Status = "ok", Message = "Curso atualizado com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do curso criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]CourseModel course)
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

            var customerId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
            if (customerId == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            if (!int.TryParse(customerId.Value, out int idAdmin))
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            string error = null;
            var model = new JsonModel();

            var item = course.GetEntity();
            item.IdAdminUser = idAdmin;
            if (idOperation.HasValue)
                item.IdOperation = idOperation.Value;


            if (!string.IsNullOrEmpty(course.Description))
            {
                var detail = course.GetDescription();
                if (staticTextRepo.Create(detail, out error))
                    item.IdDescription = detail.Id;
            }
            if (repo.Create(item, out error))
            {
                if(course.PeriodIds != null && course.PeriodIds.Length > 0)
                {
                    foreach (var periodId in course.PeriodIds)
                        repo.AddPeriod(item.Id, periodId, out error);
                }
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Curso criado com sucesso!", Id = item.Id });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o curso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            if (repo.Delete(id, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Curso removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// lista os endereço de um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageitems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">ordenação campos (id, name, street, city, state), direção (asc, desc)</param>
        /// <param name="searchword">palavra à ser buscada</param>
        /// <returns>lista com os endereços encontradas</returns>
        /// <response code="200">retorna a list, ou algum erro caso interno</response>
        /// <response code="204">se não encontrar nada</response>
        /// <response code="400">se ocorrer algum erro</response>
        [HttpGet("{id}/address")]
        [ProducesResponseType(typeof(ResultPageModel<AddressModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Listaddress(int id, [FromQuery]int page = 0, [FromQuery]int pageitems = 30, [FromQuery]string sort = "title asc", [FromQuery]string searchword = "")
        {
            var list = addressRepo.ListByCourse(id, page, pageitems, searchword, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<AddressModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<AddressModel>()
                };
                foreach (var addr in list.Page)
                    ret.Data.Add(new AddressModel(addr));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um endereço a um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um curso com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Address/{idAddress}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress(int id, int idAddress)
        {
            if (repo.AddAddress(id, idAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um endereço de um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Address/{idAddress}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            if (repo.DeleteAddress(id, idAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os ids dos períodos vínculados ao curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <returns>Lista com os Ids dos períodos</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Periods")]
        [ProducesResponseType(typeof(JsonDataModel<List<int>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPeriods(int id)
        {
            var list = repo.ListPeriods(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<int>>()
                {
                    Data = list
                };

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um período à um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="idPeriod">id do período</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um benefício com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Period/{idPeriod}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddPeriod(int id, int idPeriod)
        {
            if (repo.AddPeriod(id, idPeriod, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Período adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um período de um curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="idPeriod">id do período</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Category/{idPeriod}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveCategory(int id, int idPeriod)
        {
            if (repo.DeletePeriod(id, idPeriod, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Peíodo removida com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// lista os itens procurados
        /// </summary>
        /// <returns>lista com os itens buscados</returns>
        /// <response code="200">retorna a list, ou algum erro caso interno</response>
        /// <response code="204">se não encontrar nada</response>
        /// <response code="400">se ocorrer algum erro</response>
        [HttpGet("items/{idType}/{idOperation}")]
        [ProducesResponseType(typeof(JsonDataModel<List<ListItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListItems(int idType, int idOperation)
        {
            var list = staticTextRepo.ListByType(idType, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || !list.Any())
                    return NoContent();

                var ret = new JsonDataModel<List<ListItem>>()
                {
                    Data = new List<ListItem>()
                };
                foreach (var text in list)
                    ret.Data.Add(new ListItem(text));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa ou inativa o Curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <param name="active">true para ativar - false para inativar</param>
        /// <returns>JsonModel</returns>
        /// <response code="200"></response>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPost("ChangeActive/{id}/{active}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ChangeActive(int id, bool active)
        {
            if (repo.ChangeActive(id, active, out string error))
            {
                return Ok(new JsonModel() { Status = "ok" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Duplica um Curso
        /// </summary>
        /// <param name="id">id do curso</param>
        /// <returns>JsonModel</returns>
        /// <response code="200"></response>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPost("Duplicate/{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Duplicate(int id)
        {
            if (repo.Duplicate(id, out int newId, out string error))
            {
                return Ok(new JsonModel() { Status = "ok", Message = $"{newId}" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}