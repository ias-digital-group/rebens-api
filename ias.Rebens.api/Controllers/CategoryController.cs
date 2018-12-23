using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // GET: api/Category
        [HttpGet]
        public JsonResult Get([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var list = repo.ListPage(page, pageItems, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = list;
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }
            
            return new JsonResult(model);
        }
        
        // GET: api/Category/5
        [HttpGet("{id}", Name = "Get")]
        public JsonResult Get(int id)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var category = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = category;
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        // POST: api/Category
        [HttpPost]
        public JsonResult Post([FromBody] Category category)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var model = new JsonModel();

            if (repo.Update(category, out string error))
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

        // PUT: api/Category/5
        [HttpPut]
        public JsonResult Put([FromBody] Category category)
        {
            var repo = ServiceLocator<ICategoryRepository>.Create();
            var model = new JsonModel();

            if(repo.Create(category, out string error))
            {
                model.Status = "ok";
                model.Message = "Categoria criada com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        // DELETE: api/ApiWithActions/5
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
    }
}
