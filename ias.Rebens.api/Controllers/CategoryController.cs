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
    [Route("api/Category"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Lista as categorias com patinação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ListCategory([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var list = repo.ListPage(page, pageItems, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new ResultPageModel<CategoryModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Page = new List<CategoryModel>();
                foreach (var cat in list.Page)
                    ret.Page.Add(new CategoryModel(cat));

                model.Status = "ok";
                model.Extra = ret;
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }
            
            return new JsonResult(model);
        }
        
        /// <summary>
        /// Retorna uma categoria
        /// </summary>
        /// <param name="id">id da categoria</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public JsonResult GetCategory(int id)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var category = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = new CategoryModel(category);
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
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Post([FromBody] CategoryModel category)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
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

            return new JsonResult(model);
        }

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult Put([FromBody] CategoryModel category)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var model = new JsonModel();

            var cat = category.GetEntity();
            if(repo.Create(cat, out string error))
            {
                model.Status = "ok";
                model.Message = "Categoria criada com sucesso!";
                model.Extra = new { id = cat.Id };
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
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

            return new JsonResult(model);

        }

        /// <summary>
        /// Lista a árvore de categorias
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListTree")]
        public JsonResult ListTree()
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var list = repo.ListTree(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<CategoryModel>();
                list.ForEach(item => { ret.Add(new CategoryModel(item)); });

                model.Status = "ok";
                model.Extra = ret;
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
