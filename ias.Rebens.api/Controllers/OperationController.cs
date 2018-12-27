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
    [Route("api/Operation")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        [HttpGet]
        public JsonResult ListOperation([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var repo = ServiceLocator<IOperationRepository>.Create();
            var list = repo.ListPage(page, pageItems, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                var ret = new ResultPageModel<OperationModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Page = new List<OperationModel>();
                foreach (var operation in list.Page)
                    ret.Page.Add(new OperationModel(operation));

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
        public JsonResult GetOperation(int id)
        {
            var repo = ServiceLocator<IOperationRepository>.Create();
            var operation = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Extra = new OperationModel(operation);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        [HttpPost]
        public JsonResult Post([FromBody] OperationModel operation)
        {
            var repo = ServiceLocator<IOperationRepository>.Create();
            var model = new JsonModel();
            string error = null;

            var op = operation.GetEntity();
            if (operation.Contact != null)
            {
                var contact = operation.Contact.GetEntity();

                if (operation.Contact.Address != null)
                {
                    var addrRepo = ServiceLocator<IAddressRepository>.Create();
                    var addr = operation.Contact.Address.GetEntity();
                    if (addrRepo.Update(addr, out error))
                        contact.IdAddress = addr.Id;
                }

                if (string.IsNullOrEmpty(error))
                {
                    var contactRepo = ServiceLocator<IContactRepository>.Create();
                    if (contactRepo.Update(contact, out error))
                        op.IdContact = contact.Id;
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                model.Status = "error";
                model.Message = error;
            }
            else
            {
                if (repo.Update(op, out error))
                {
                    model.Status = "ok";
                    model.Message = "Operação atualizada com sucesso!";
                }
                else
                {
                    model.Status = "error";
                    model.Message = error;
                }
            }

            return new JsonResult(model);
        }

        [HttpPut]
        public JsonResult Put([FromBody] OperationModel operation)
        {
            var repo = ServiceLocator<IOperationRepository>.Create();
            var model = new JsonModel();
            string error = null;

            var op = operation.GetEntity();
            if(operation.Contact != null )
            {
                var contact = operation.Contact.GetEntity();

                if(operation.Contact.Address != null)
                {
                    var addrRepo = ServiceLocator<IAddressRepository>.Create();
                    var addr = operation.Contact.Address.GetEntity();
                    if (addrRepo.Create(addr, out error))
                        contact.IdAddress = addr.Id;
                }

                if(string.IsNullOrEmpty(error))
                {
                    var contactRepo = ServiceLocator<IContactRepository>.Create();
                    if (contactRepo.Create(contact, out error))
                        op.IdContact = contact.Id;
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                model.Status = "error";
                model.Message = error;
            }
            else
            {
                if (repo.Create(op, out error))
                {
                    model.Status = "ok";
                    model.Message = "Operação criada com sucesso!";
                    model.Extra = new { id = op.Id };
                }
                else
                {
                    model.Status = "error";
                    model.Message = error;
                }
            }

            return new JsonResult(model);
        }
    }
}