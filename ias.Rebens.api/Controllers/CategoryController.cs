using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/Category"), Authorize("Bearer", Roles = "administrator,test")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryRepository repo;
        private ILogErrorRepository logRepo;

        public CategoryController(ICategoryRepository categoryRepository, ILogErrorRepository logErrorRepository) {
            this.repo = categoryRepository;
            this.logRepo = logErrorRepository;
        }

        /// <summary>
        /// Retorna uma lista de categorias conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListCategory([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new ResultPageModel<CategoryModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<CategoryModel>();
                foreach (var cat in list.Page)
                    ret.Data.Add(new CategoryModel(cat));

                return Ok(ret);
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Retorna a categoria conforme o ID
        /// </summary>
        /// <param name="id">Id da categoria desejada</param>
        /// <returns>Categoria</returns>
        /// <response code="201">Retorna a categoria, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (category == null || category.Id == 0)
                    return NoContent();
                return Ok(new { data = new CategoryModel(category) });
            }

            return Ok(new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma categoria
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody]CategoryModel category)
        {
            var model = new JsonModel();
            if (repo.Update(category.GetEntity(), out string error))
            {
                model.Status = "ok";
                model.Message = "Categoria atualizada com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Cria uma categoria
        /// </summary>
        /// <param name="category"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da categoria criada</returns>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult Post([FromBody]CategoryModel category)
        {
            var model = new JsonModel();
            var cat = category.GetEntity();
            if(repo.Create(cat, out string error))
            {
                model.Status = "ok";
                model.Message = "Categoria criada com sucesso!";
                model.Data = new { id = cat.Id };
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Apaga uma categoria
        /// </summary>
        /// <param name="id">Id da categoria a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = new JsonModel();
            if (repo.Delete(id, out string error))
            {
                model.Status = "ok";
                model.Message = "Categoria excluida com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);

        }

        /// <summary>
        /// Retorna a arvore de categorias 
        /// </summary>
        /// <returns>Lista com as categorias encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("ListTree")]
        public IActionResult ListTree()
        {
            var list = repo.ListTree(out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new List<CategoryModel>();
                list.ForEach(item => { ret.Add(new CategoryModel(item)); });

                return Ok(ret);
            }
            else
            {
                var model = new JsonModel();
                model.Status = "error";
                model.Message = error;
                return Ok(model);
            }
        }
    }
}
