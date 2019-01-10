using System.Collections.Generic;
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

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="benefitRepository">Injeção de dependencia do repositório de Benefício</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de Endereço</param>
        /// <param name="categoryRepository">Injeção de dependencia do repositório de Categoria</param>
        public BenefitController(IBenefitRepository benefitRepository, IAddressRepository addressRepository, ICategoryRepository categoryRepository)
        {
            this.repo = benefitRepository;
            this.addressRepo = addressRepository;
            this.categoryRepo = categoryRepository;
        }

        /// <summary>
        /// Lista todos os benefícios com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os benefícios encontrados</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListBenefit([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
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

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o benefício conforme o ID
        /// </summary>
        /// <param name="id">Id do benefício</param>
        /// <returns>Parceiros</returns>
        /// <response code="201">Retorna o benefício, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetBenefit(int id)
        {
            var benefit = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (benefit == null || benefit.Id == 0)
                    return NoContent();
                return Ok(new { data = new BenefitModel(benefit) });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um benefício
        /// </summary>
        /// <param name="benefit"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody]BenefitModel benefit)
        {
            var model = new JsonModel();
            string error = null;

            var part = benefit.GetEntity();
            if (repo.Update(part, out error))
            {
                model.Status = "ok";
                model.Message = "Benefício atualizado com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Cria um benefício
        /// </summary>
        /// <param name="benefit"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do benefício criado</returns>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult Post([FromBody]BenefitModel benefit)
        {
            string error = null;
            var model = new JsonModel();

            var item = benefit.GetEntity();
            if (repo.Create(item, out error))
            {
                model.Status = "ok";
                model.Message = "Benefício criado com sucesso!";
                model.Data = new { id = item.Id };
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }
            return Ok(model);
        }

        /// <summary>
        /// Lista os endereço de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Address")]
        public IActionResult ListAddress(int id)
        {
            var list = addressRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<AddressModel>();
                foreach (var addr in list)
                    ret.Add(new AddressModel(addr));

                return Ok(new { data = ret });
            }

            var model = new JsonModel()
            {
                Status = "error",
                Message = error
            };
            return Ok(model);
        }

        /// <summary>
        /// Adiciona um endereço a um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddAddress")]
        public IActionResult AddAddress([FromBody]BenefitAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdBenefit, model.IdAddress, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Endereço adicionado com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Remove um endereço de um parceiro
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("RemoveAddress")]
        public IActionResult RemoveAddress([FromBody]BenefitAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteAddress(model.IdBenefit, model.IdAddress, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Endereço removido com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Lista as categorias de um benefício
        /// </summary>
        /// <param name="id">id do benefício</param>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Category")]
        public IActionResult ListCategories(int id)
        {
            var list = addressRepo.ListByBenefit(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<AddressModel>();
                foreach (var addr in list)
                    ret.Add(new AddressModel(addr));

                return Ok(new { data = ret });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona uma categoria a um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, IdCategory: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody]BenefitCategoryModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddCategory(model.IdBenefit, model.IdCategory, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Categoria adicionada com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Remove uma categoria de um benefício
        /// </summary>
        /// <param name="model">{ IdBenefit: 0, IdCategory: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("RemoveCategory")]
        public IActionResult RemoveCategory([FromBody]BenefitCategoryModel model)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteCategory(model.IdBenefit, model.IdCategory, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Categoria removida com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }
    }
}