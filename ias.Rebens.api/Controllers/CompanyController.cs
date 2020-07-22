using System.Collections.Generic;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens")]
    [ApiController]
    public class CompanyController : BaseApiController
    {
        private ICompanyRepository repo;
        private IAddressRepository addressRepo;
        private IContactRepository contactRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="companyRepository"></param>
        public CompanyController(ICompanyRepository companyRepository, IAddressRepository addressRepository, IContactRepository contactRepository)
        {
            this.repo = companyRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;

        }

        /// <summary>
        /// Lista as empresas conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">Active, não obrigatório (default=null)</param>
        /// <param name="type">Tipo de banner, não obrigatório (default=null)</param>
        /// <param name="idOperation">id da Operação, não obrigatório (default=null)</param>
        /// <param name="idPartner">id da Operação, não obrigatório (default=null)</param>
        /// <returns>Lista com as empresas encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<CompanyModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery] int page = 0, [FromQuery] int pageItems = 30, [FromQuery] string sort = "Title ASC",
            [FromQuery] string searchWord = "", [FromQuery] bool? active = null, [FromQuery] int? type = null, 
            [FromQuery] int? idOperation = null, [FromQuery] int? idPartner = null)
        {
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, type, type == (int)Enums.LogItem.Operation ? idOperation : idPartner, active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<CompanyModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<CompanyModel>()
                };
                foreach (var company in list.Page)
                    ret.Data.Add(new CompanyModel(company));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma empresa
        /// </summary>
        /// <param name="id">Id da empresa desejada</param>
        /// <returns>Banner</returns>
        /// <response code="200">Retorna a empresa, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<CompanyModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var company = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (company == null || company.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<CompanyModel>() { Data = new CompanyModel(company) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }


        /// <summary>
        /// Ativa/Inativa uma empresa
        /// </summary>
        /// <param name="id">id da empresa</param>
        /// <returns></returns>
        /// <response code="200">Se o tudo ocorrer sem erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/ToggleActive")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ToggleActive(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var status = repo.ToggleActive(id, idAdminUser, out string error);

            if (string.IsNullOrEmpty(error))
                return Ok(new JsonModel() { Status = "ok", Data = status });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma empreas
        /// </summary>
        /// <param name="company"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody] CompanyModel company)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            string error = null;

            var co = company.GetEntity();
            if (company.Address != null)
            {
                if (company.Address.Id > 0)
                    addressRepo.Update(company.Address.GetEntity(), idAdminUser, out _);
                else
                {
                    var addr = company.Address.GetEntity();
                    if (addressRepo.Create(addr, idAdminUser, out _))
                        co.IdAddress = addr.Id;
                }
            }
            if (company.Contact != null)
            {
                if (company.Contact.Id > 0)
                    contactRepo.Update(company.Contact.GetEntity(), idAdminUser, out _);
                else
                {
                    var contact = company.Contact.GetEntity();
                    if (contactRepo.Create(contact, idAdminUser, out _))
                        co.IdContact = contact.Id;
                }
            }

            if (repo.Update(co, idAdminUser, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Empresa atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma empresa
        /// </summary>
        /// <param name="company"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da empresa criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] CompanyModel company)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            string error = null;
            if (company.Contact != null)
            {
                var contact = company.Contact.GetEntity();
                if (contactRepo.Create(contact, idAdminUser, out error))
                    company.IdContact = contact.Id;
            }
            if (company.Address != null)
            {
                var addr = company.Address.GetEntity();
                if (addressRepo.Create(addr, idAdminUser, out error))
                    company.IdAddress = addr.Id;
            }

            if (!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            var co = company.GetEntity();
            if (repo.Create(co, idAdminUser, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Empresa criada com sucesso!", Id = co.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga uma empresa
        /// </summary>
        /// <param name="id">Id da empresa a ser apagado</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Empresa apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
