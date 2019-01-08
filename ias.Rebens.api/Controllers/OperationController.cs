using System;
using System.Collections.Generic;
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
        private IAddressRepository addrRepo;
        private IContactRepository contactRepo;

        public OperationController(IOperationRepository operationRepository, IContactRepository contactRepository, IAddressRepository addressRepository)
        {
            this.repo = operationRepository;
            this.addrRepo = addressRepository;
            this.contactRepo = contactRepository;
        }

        /// <summary>
        /// Lista todas as operações com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Domain, Title, CompanyName, CompanyDoc), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListOperation([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            string op = TokenHelper.GetCurrentUser(User.Identity);
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

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

                return Ok(ret);
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Retorna a operação conforme o ID
        /// </summary>
        /// <param name="id">Id da operação desejada</param>
        /// <returns>Operação</returns>
        /// <response code="201">Retorna a operação, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetOperation(int id)
        {
            var operation = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (operation == null || operation.Id == 0)
                    return NoContent();
                return Ok(new { data = new OperationModel(operation) });
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Atualiza uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody]OperationModel operation)
        {
            var model = new JsonModel();
            string error = null;

            var op = operation.GetEntity();
            if (operation.Contact != null)
            {
                var contact = operation.Contact.GetEntity();

                if (operation.Contact.Address != null)
                {
                    var addr = operation.Contact.Address.GetEntity();
                    if (addrRepo.Update(addr, out error))
                        contact.IdAddress = addr.Id;
                }

                if (string.IsNullOrEmpty(error))
                {
                    contactRepo.Update(contact, out error);
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
        /// <param name="category"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da operação criada</returns>
        /// <response code="201"></response>
        [HttpPost]
        public IActionResult Post([FromBody] OperationModel operation)
        {
            string error = null;
            int idContact = 0;
            var model = new JsonModel();

            var op = operation.GetEntity();
            if(operation.Contact != null )
            {
                var contact = operation.Contact.GetEntity();

                if(operation.Contact.Address != null)
                {
                    var addr = operation.Contact.Address.GetEntity();
                    if (addrRepo.Create(addr, out error))
                    {
                        contact.IdAddress = addr.Id;
                        if (contactRepo.Create(contact, out error))
                        {
                            idContact = contact.Id;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                model.Status = "error";
                model.Message = error;
            }
            else {

                if (repo.Create(op, out error))
                {
                    if (idContact > 0)
                    {
                        if (repo.AddContact(op.Id, idContact, out error))
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
                    else
                    {
                        model.Status = "ok";
                        model.Message = "Operação criada com sucesso!";
                        model.Data = new { id = op.Id };
                    }
                    
                }
                else
                {
                    model.Status = "error";
                    model.Message = error;
                }
            }
            return Ok(model);

        }
    }
}