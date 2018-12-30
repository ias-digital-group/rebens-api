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
    [Route("api/[controller]"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        /// <summary>
        /// Lista os tipos de benefício
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListBenefitType")]
        public JsonResult ListBenefitType()
        {
            var repo = ServiceLocator<IBenefitTypeRepository>.Create();
            var list = repo.ListActive(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<BenefitTypeModel>();
                list.ForEach(item => { ret.Add(new BenefitTypeModel(item)); });

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
        /// Lista os tipos de integração
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListIntegrationType")]
        public JsonResult ListIntegrationType()
        {
            var repo = ServiceLocator<IIntegrationTypeRepository>.Create();
            var list = repo.ListActive(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<IntegrationTypeModel>();
                list.ForEach(item => { ret.Add(new IntegrationTypeModel(item)); });

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
        /// Lista os tipos de operação
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListOperationType")]
        public JsonResult ListOperationType()
        {
            var repo = ServiceLocator<IOperationTypeRepository>.Create();
            var list = repo.ListActive(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<OperationTypeModel>();
                list.ForEach(item => { ret.Add(new OperationTypeModel(item)); });

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
        /// Lista os tipos de textos estáticos
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListStaticTextType")]
        public JsonResult ListStaticTextType()
        {
            var repo = ServiceLocator<IStaticTextTypeRepository>.Create();
            var list = repo.ListActive(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<StaticTextTypeModel>();
                list.ForEach(item => { ret.Add(new StaticTextTypeModel(item)); });

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
        /// Lista as permissões do sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListPermissionsTree")]
        public JsonResult ListPermissionsTree()
        {
            var repo = ServiceLocator<IPermissionRepository>.Create();
            var list = repo.ListTree(out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new List<PermissionModel>();
                list.ForEach(item => { ret.Add(new PermissionModel(item)); });

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