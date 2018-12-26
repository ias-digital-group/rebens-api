using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/Faq")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        [HttpGet]
        public JsonResult ListFaq([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var repo = ServiceLocator<IFaqRepository>.Create();
            var list = repo.ListPage(page, pageItems, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new ResultPageModel<FaqModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Page = new List<FaqModel>();
                foreach (var faq in list.Page)
                    ret.Page.Add(new FaqModel(faq));

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

        [HttpGet("{id}")]
        public JsonResult GetFaq(int id)
        {
            var repo = ServiceLocator<IFaqRepository>.Create();
            var faq = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = new FaqModel(faq);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult Post([FromBody] FaqModel faq)
        {
            var repo = ServiceLocator<IFaqRepository>.Create();
            var model = new JsonModel();

            if (repo.Update(faq.GetEntity(), out string error))
            {
                model.Status = "ok";
                model.Message = "Pergunta atualizada com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPut]
        public JsonResult Put([FromBody] FaqModel faq)
        {
            var repo = ServiceLocator<IFaqRepository>.Create();
            var model = new JsonModel();

            if (repo.Create(faq.GetEntity(), out string error))
            {
                model.Status = "ok";
                model.Message = "Pergunta criada com sucesso!";
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
            var repo = ServiceLocator<IFaqRepository>.Create();
            var model = new JsonModel();

            if (repo.Delete(id, out string error))
            {
                model.Status = "ok";
                model.Message = "Pergunta excluida com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);

        }

        [HttpGet("ListByOperation")]
        public JsonResult ListByOperation([FromQuery]int idOperation)
        {
            var repo = ServiceLocator<IFaqRepository>.Create();
            var list = repo.ListByOperation(idOperation, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<FaqModel>();
                list.ForEach(item => { ret.Add(new FaqModel(item)); });

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