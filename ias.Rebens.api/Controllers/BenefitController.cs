using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ias.Rebens.api.Models;
using System;
using System.Linq;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Benefit Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publisher,administratorRebens,publisherRebens,couponChecker")]
    [ApiController]
    public class BenefitController : BaseApiController
    {
        private readonly IBenefitRepository repo;
        private readonly IBenefitUseRepository benefitUseRepo;
        private readonly IAddressRepository addressRepo;
        private readonly IStaticTextRepository staticTextRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IOperationRepository operationRepo;
        private readonly Constant constant;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="benefitRepository">Injeção de dependencia do repositório de Benefício</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de Endereço</param>
        /// <param name="categoryRepository">Injeção de dependencia do repositório de Categoria</param>
        /// <param name="operationRepository">Injeção de dependencia do repositório de Operação</param>
        /// <param name="staticTextRepository">Injeção de dependencia do repositório de Texto Estático</param>
        /// <param name="benefitUseRepository">Injeção de dependencia do repositório de Uso de beneficio</param>
        public BenefitController(IBenefitRepository benefitRepository, IAddressRepository addressRepository, ICategoryRepository categoryRepository, 
            IOperationRepository operationRepository, IStaticTextRepository staticTextRepository, IBenefitUseRepository benefitUseRepository)
        {
            this.repo = benefitRepository;
            this.addressRepo = addressRepository;
            this.categoryRepo = categoryRepository;
            this.operationRepo = operationRepository;
            this.staticTextRepo = staticTextRepository;
            this.benefitUseRepo = benefitUseRepository;
            this.constant = new Constant();
        }

        /// <summary>
        /// Lista todos os benefícios com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">id da operação, não obrigatório (default=null)</param>
        /// <param name="active">status, não obrigatório (default=null)</param>
        /// <param name="type">tipo do benefício, não obrigatório (default=null)</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "",
            [FromQuery]int? idOperation = null, [FromQuery]bool? active = null, [FromQuery]int? type = null)
        {
            bool exclusive = false;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                exclusive = true;
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation, active, type, exclusive);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BenefitModel>()
                };
                foreach (var benefit in list.Page)
                    ret.Data.Add(new BenefitModel(this.constant.URL, benefit));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o benefício conforme o ID
        /// </summary>
        /// <param name="id">Id do benefício</param>
        /// <returns>Parceiros</returns>
        /// <response code="200">Retorna o benefício, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var benefit = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (benefit == null || benefit.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<BenefitModel>() { Data = new BenefitModel(this.constant.URL, benefit) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um benefício
        /// </summary>
        /// <param name="benefit"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]BenefitModel benefit)
        {
            if (benefit == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto não pode ser nulo!" });

            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var part = benefit.GetEntity();
            if (repo.Update(part, idAdminUser, out string error))
            {
                if (!string.IsNullOrEmpty(benefit.Detail))
                {
                    var detail = benefit.GetDetail();
                    staticTextRepo.Update(detail, idAdminUser, out error);
                }
                if (!string.IsNullOrEmpty(benefit.HowToUse))
                {
                    var howToUse = benefit.GetHowToUse();
                    staticTextRepo.Update(howToUse, idAdminUser, out error);
                }
                if (repo.ConnectOperations(part.Id, benefit.Operations, out error) && 
                    repo.ConnectCategories(part.Id, benefit.Categories, out error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Benefício atualizado com sucesso!", Id = part.Id });
                else
                    return Ok(new JsonCreateResultModel() { Status = "error", Message = error, Id = part.Id });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um benefício
        /// </summary>
        /// <param name="benefit"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do benefício criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]BenefitModel benefit)
        {
            if(benefit == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Objeto não pode ser nulo!" });

            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher" }))
            {
                idOperation = GetOperationId(out string errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            int idAdminUser = GetAdminUserId(out string errorUser);
            if (errorUser != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorUser });

            var item = benefit.GetEntity();
            if (idOperation.HasValue)
            {
                item.Exclusive = true;
                item.IdOperation = idOperation.Value;
                item.IdIntegrationType = (int)Enums.IntegrationType.Own;
            }

            if (repo.Create(item, idAdminUser, out string error))
            {
                if (!string.IsNullOrEmpty(benefit.Detail))
                {
                    var detail = benefit.GetDetail();
                    detail.IdBenefit = item.Id;
                    staticTextRepo.Create(detail, out error);
                }
                if (!string.IsNullOrEmpty(benefit.HowToUse))
                {
                    var howToUse = benefit.GetHowToUse();
                    howToUse.IdBenefit = item.Id;
                    staticTextRepo.Create(howToUse, out error);
                }

                if (repo.ConnectOperations(item.Id, benefit.Operations, out error) &&
                    repo.ConnectCategories(item.Id, benefit.Categories, out error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Benefício criado com sucesso!", Id = item.Id });
                else
                    return Ok(new JsonCreateResultModel() { Status = "error", Message = error, Id = item.Id });
            }
                
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um Benefício
        /// </summary>
        /// <param name="id">id do benefídio</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o benefício</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Benefício removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os endereço de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Address")]
        [ProducesResponseType(typeof(ResultPageModel<AddressModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListAddress(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = addressRepo.ListByBenefit(id, page, pageItems, searchWord, sort, out string error);

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
        /// Adiciona um endereço a um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um benefício com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddAddress")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress([FromBody]BenefitAddressModel model)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.AddAddress(model.IdBenefit, model.IdAddress, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço adicionado com sucesso!" });
            
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um endereço de um parceiro
        /// </summary>
        /// <param name="id">id do benefídio</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Address/{idAddress}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.DeleteAddress(id, idAddress, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os ids das categorias vínculadas ao benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com os Ids das categorias</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Category")]
        [ProducesResponseType(typeof(JsonDataModel<List<int>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCategories(int id)
        {
            var list = categoryRepo.ListByBenefit(id, out string error);

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
        /// Adiciona uma categoria a um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, IdCategory: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um benefício com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SaveCategories")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveCategories([FromBody]BenefitCategoriesModel model)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.SaveCategories(model.IdBenefit, model.CategoryIds, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categorias salvas com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todas operações, e marca quais estão vinculadas ao benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com todas operações</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Operations")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitOperationItemModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListOperations(int id)
        {
            var list = operationRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BenefitOperationItemModel>>();
                ret.Data = new List<BenefitOperationItemModel>();
                foreach (var item in list)
                    ret.Data.Add(new BenefitOperationItemModel(item));

                return Ok(ret);
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as posições
        /// </summary>
        /// <returns>Lista com as posições</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Positions")]
        [ProducesResponseType(typeof(JsonDataModel<List<PositionModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPositions()
        {
            var list = repo.ListPositions(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<PositionModel>>();
                ret.Data = new List<PositionModel>();
                foreach (var pos in list)
                    ret.Data.Add(new PositionModel(pos));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os benefícios ativos
        /// </summary>
        /// <returns>Lista com os benefícios</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Active")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitItem>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListActive()
        {
            var list = repo.ListActive(out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BenefitItem>>();
                ret.Data = new List<BenefitItem>();
                foreach (var benefit in list)
                    ret.Data.Add(new BenefitItem(benefit));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Ativa ou inativa o Benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <param name="active">true para ativar - false para inativar</param>
        /// <returns>JsonModel</returns>
        /// <response code="200"></response>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPost("ToggleActive/{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(204)]
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
        /// Duplica um Benefício
        /// </summary>
        /// <param name="id">id do Benefício</param>
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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Duplicate(id, out int newId, idAdminUser, out string error))
            {
                return Ok(new JsonModel() { Status = "ok", Message = $"{newId}" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        
        /// <summary>
        /// Lista os vouchers Varejo Local para validaçaõ
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idPartner">id do parceiro</param>
        /// <returns>Lista com os vouchers encontrados</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("UseValidation")]
        [ProducesResponseType(typeof(ResultPageModel<BenefitUseListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UseValidation([FromQuery] int page = 0, [FromQuery] int pageItems = 30, [FromQuery] string searchWord = "", [FromQuery]int? idPartner = null)
        {
            if (CheckRole(Enums.Roles.couponChecker.ToString()))
            {
                idPartner = GetPartnerId(out string errorId);
                if(!string.IsNullOrEmpty(errorId))
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            var list = benefitUseRepo.ValidateListPage(page, pageItems, searchWord, out string error, idPartner);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitUseListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BenefitUseListItemModel>()
                };
                foreach (var b in list.Page)
                    ret.Data.Add(new BenefitUseListItemModel(b));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Valida um cupom
        /// </summary>
        /// <param name="id">id do cupom</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok</returns>
        /// <response code="200">Se o objeto for ataulizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("Validate/{id}")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Validate(int id)
        {
            int idAdminUser = GetAdminUserId(out string error);
            if(!string.IsNullOrEmpty(error))
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            if (benefitUseRepo.SetUsed(id, idAdminUser, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Item validado com sucesso!" });
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}