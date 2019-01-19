﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BenefitController : ControllerBase
    {
        private IBenefitRepository repo;
        private IAddressRepository addressRepo;
        private ICategoryRepository categoryRepo;
        private IOperationRepository operationRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="benefitRepository">Injeção de dependencia do repositório de Benefício</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de Endereço</param>
        /// <param name="categoryRepository">Injeção de dependencia do repositório de Categoria</param>
        /// <param name="operationRepository">Injeção de dependencia do repositório de Operação</param>
        public BenefitController(IBenefitRepository benefitRepository, IAddressRepository addressRepository, ICategoryRepository categoryRepository, IOperationRepository operationRepository)
        {
            this.repo = benefitRepository;
            this.addressRepo = addressRepository;
            this.categoryRepo = categoryRepository;
            this.operationRepo = operationRepository;
        }

        /// <summary>
        /// Lista todos os benefícios com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<BenefitModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BenefitModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BenefitModel>();
                foreach (var benefit in list.Page)
                    ret.Data.Add(new BenefitModel(benefit));

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
                return Ok(new JsonDataModel<BenefitModel>() { Data = new BenefitModel(benefit) });
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
            var model = new JsonModel();
            string error = null;

            var part = benefit.GetEntity();
            if (repo.Update(part, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Benefício atualizado com sucesso!" });

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
            string error = null;
            var model = new JsonModel();

            var item = benefit.GetEntity();
            if (repo.Create(item, out error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Benefício criado com sucesso!", Id = item.Id });
            
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os endereço de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Address")]
        [ProducesResponseType(typeof(JsonDataModel<List<AddressModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListAddress(int id)
        {
            var list = addressRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<AddressModel>>();
                ret.Data = new List<AddressModel>();
                foreach (var addr in list)
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
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdBenefit, model.IdAddress, out string error))
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
            var resultModel = new JsonModel();

            if (repo.DeleteAddress(id, idAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as categorias de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Category")]
        [ProducesResponseType(typeof(JsonDataModel<List<CategoryModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCategories(int id)
        {
            var list = categoryRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<CategoryModel>>();
                ret.Data = new List<CategoryModel>();
                foreach (var cat in list)
                    ret.Data.Add(new CategoryModel(cat));

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
        [HttpPost("AddCategory")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddCategory([FromBody]BenefitCategoryModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddCategory(model.IdBenefit, model.IdCategory, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove uma categoria de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <param name="idCategory">id da categoria</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com uma categoria</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Category/{idCategory}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveCategory(int id, int idCategory)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteCategory(id, idCategory, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Categoria removida com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as operações de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Operations")]
        public IActionResult ListOperations(int id)
        {
            var list = operationRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<OperationModel>();
                foreach (var op in list)
                    ret.Add(new OperationModel(op));

                return Ok(new { data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona uma operação a um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, id: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um benefício com uma Operação</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddOperation")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddOperation([FromBody]BenefitOperationModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddOperation(model.IdBenefit, model.IdOperation, model.IdPosition, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Operação adicionada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove uma operação de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <param name="idOperation">id da categoria</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com uma operação</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Operation/{idOperation}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveOperation(int id, int idOperation)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteOperation(id, idOperation, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Operação removida com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as posições
        /// </summary>
        /// <returns>Lista com as posições</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
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
    }
}