using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/Faq"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private IFaqRepository repo;

        public FaqController(IFaqRepository faqRepository, IContactRepository contactRepository, IAddressRepository addressRepository)
        {
            this.repo = faqRepository;
        }

        /// <summary>
        /// Lista as perguntas de faq conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Question, Answer, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as faqs encontradas</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListFaq([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Question ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count() == 0)
                    return NoContent();

                var ret = new ResultPageModel<FaqModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<FaqModel>();
                foreach (var faq in list.Page)
                    ret.Data.Add(new FaqModel(faq));

                return Ok(ret);
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Retorna uma pergunta
        /// </summary>
        /// <param name="id">Id da pergunta desejada</param>
        /// <returns>FAQ</returns>
        /// <response code="201">Retorna a faq, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetFaq(int id)
        {
            var faq = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (faq == null || faq.Id == 0)
                    return NoContent();
                return Ok(new { data = new FaqModel(faq) });
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Atualiza uma pergunta
        /// </summary>
        /// <param name="faq">FAQ</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody] FaqModel faq)
        {
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

            return Ok(model);
        }

        /// <summary>
        /// Cria uma pergunta
        /// </summary>
        /// <param name="faq"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult Post([FromBody] FaqModel faq)
        {
            var model = new JsonModel();

            var f = faq.GetEntity();
            if (repo.Create(f, out string error))
            {
                model.Status = "ok";
                model.Message = "Pergunta criada com sucesso!";
                model.Data = new { id = f.Id };
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Apaga uma pergunta
        /// </summary>
        /// <param name="id">Id da pergunta a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
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

            return Ok(model);

        }
    }
}