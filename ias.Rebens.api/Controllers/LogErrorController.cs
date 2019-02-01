using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// LogError Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogErrorController : ControllerBase
    {
        private ILogErrorRepository repo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logErrorRepository"></param>
        public LogErrorController(ILogErrorRepository logErrorRepository)
        {
            this.repo = logErrorRepository;
        }

        /// <summary>
        /// Lista os logs com paginação
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <param name="pageItems"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List([FromQuery]string token, [FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var model = new JsonModel();
            if (string.IsNullOrEmpty(token))
            {
                model.Status = "error";
                model.Message = "wrong token";
            }
            else
            {
                var decryptedToken = Helper.SecurityHelper.SimpleDecryption(token);
                if(decryptedToken.IndexOf('|') > 0 && decryptedToken.Split('|').Length == 2 && decryptedToken.Split('|')[0] == "ias_user" && decryptedToken.Split('|')[1] == "#K)YKb4B=&eN")
                {
                    var list = repo.ListPage(page, pageItems);

                    var ret = new ResultPageModel<LogError>();
                    ret.CurrentPage = list.CurrentPage;
                    ret.HasNextPage = list.HasNextPage;
                    ret.HasPreviousPage = list.HasPreviousPage;
                    ret.ItemsPerPage = list.ItemsPerPage;
                    ret.TotalItems = list.TotalItems;
                    ret.TotalPages = list.TotalPages;
                    ret.Data = list.Page.ToList();

                    model.Status = "ok";
                    model.Data = ret;
                  
                }
                else
                {
                    model.Status = "error";
                    model.Message = "wrong token";
                }
            }

            return new JsonResult(model);
        }

        /// <summary>
        /// Limpa todos os logs
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult Clear([FromQuery]string token)
        {
            var model = new JsonModel();
            if (string.IsNullOrEmpty(token))
            {
                model.Status = "error";
                model.Message = "wrong token";
            }
            else
            {
                var decryptedToken = Helper.SecurityHelper.SimpleDecryption(token);
                if (decryptedToken.IndexOf('|') > 0 && decryptedToken.Split('|').Length == 2 && decryptedToken.Split('|')[0] == "ias_user" && decryptedToken.Split('|')[1] == "#K)YKb4B=&eN")
                {
                    try
                    {
                        repo.Clear();
                        model.Status = "ok";
                    }
                    catch (Exception ex)
                    {
                        model.Status = "error";
                        model.Message = ex.Message;
                    }
                }
                else
                {
                    model.Status = "error";
                    model.Message = "wrong token";
                }
            }

            return new JsonResult(model);
        }
    }
}