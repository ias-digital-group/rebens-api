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
    [Route("api/Operation"), Authorize("Bearer", Roles = "administrator")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private IOperationRepository repo;

        public OperationController(IOperationRepository operationRepository)
        {
            this.repo = operationRepository;
        }

        /// <summary>
        /// Lista todas as operações com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ListOperation([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            string op = TokenHelper.GetCurrentUser(User.Identity);
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
                ret.Data = new List<OperationModel>();
                foreach (var operation in list.Page)
                    ret.Data.Add(new OperationModel(operation));

                model.Status = "ok";
                model.Message = op;
                model.Data = ret;
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        /// <summary>
        /// Retorna uma operação
        /// </summary>
        /// <param name="id">Id da operação</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public JsonResult GetOperation(int id)
        {
            var operation = repo.Read(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                model.Status = "ok";
                model.Data = new OperationModel(operation);
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return new JsonResult(model);
        }

        /// <summary>
        /// Atualiza uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Post([FromBody] OperationModel operation)
        {
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

        /// <summary>
        /// Cria uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult Put([FromBody] OperationModel operation)
        {
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
                    model.Data = new { id = op.Id };
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