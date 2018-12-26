using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogErrorController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get([FromQuery]string token, [FromQuery]int page = 0, [FromQuery]int pageItems = 30)
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
                    var repo = ServiceLocator<ILogErrorRepository>.Create();
                    var list = repo.ListPage(page, pageItems);

                    var ret = new ResultPageModel<LogError>();
                    ret.CurrentPage = list.CurrentPage;
                    ret.HasNextPage = list.HasNextPage;
                    ret.HasPreviousPage = list.HasPreviousPage;
                    ret.ItemsPerPage = list.ItemsPerPage;
                    ret.TotalItems = list.TotalItems;
                    ret.TotalPages = list.TotalPages;
                    ret.Page = list.Page.ToList();

                    model.Status = "ok";
                    model.Extra = ret;
                  
                }
                else
                {
                    model.Status = "error";
                    model.Message = "wrong token";
                }
            }

            return new JsonResult(model);
        }

        [HttpGet("Clear")]
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
                        var repo = ServiceLocator<ILogErrorRepository>.Create();
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

        [HttpGet("DeleteOlderThan")]
        public JsonResult Delete([FromQuery]string token, [FromQuery]DateTime date)
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
                        var repo = ServiceLocator<ILogErrorRepository>.Create();
                        repo.DeleteOlderThan(date);
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