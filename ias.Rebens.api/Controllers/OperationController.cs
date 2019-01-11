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
        private IAddressRepository addressRepo;
        private IContactRepository contactRepo;
        private IFaqRepository faqRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="operationRepository">Injeção de dependencia do repositório de operação</param>
        /// <param name="contactRepository">Injeção de dependencia do repositório de contato</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de endereço</param>
        /// <param name="faqRepository">Injeção de dependencia do repositório de faq</param>
        public OperationController(IOperationRepository operationRepository, IContactRepository contactRepository, IAddressRepository addressRepository, IFaqRepository faqRepository)
        {
            this.repo = operationRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;
            this.faqRepo = faqRepository;
        }

        /// <summary>
        /// Lista todas as operações com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Domain, Title, CompanyName, CompanyDoc), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
        public IActionResult ListOperation([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

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

            var op = operation.GetEntity();
            if (repo.Update(op, out string error))
            {
                model.Status = "ok";
                model.Message = "Operação atualizada com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Cria uma operação
        /// </summary>
        /// <param name="operation"></param>
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
                    if (addressRepo.Create(addr, out error))
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

        /// <summary>
        /// Lista os contatos de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Contacts")]
        public IActionResult ListContacts(int id)
        {
            var list = contactRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<ContactModel>();
                foreach (var contact in list)
                    ret.Add(new ContactModel(contact));

                return Ok(new { data = ret });
            }

            var model = new JsonModel()
            {
                Status = "error",
                Message = error
            };
            return Ok(model);
        }

        /// <summary>
        /// Lista os endereço de uma Operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Address")]
        public IActionResult ListAddress(int id)
        {
            var list = addressRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<AddressModel>();
                foreach (var addr in list)
                    ret.Add(new AddressModel(addr));

                return Ok(new { data = ret });
            }

            var model = new JsonModel()
            {
                Status = "error",
                Message = error
            };
            return Ok(model);
        }

        /// <summary>
        /// Adiciona um contato a uma operação
        /// </summary>
        /// <param name="model">{ idOperation: 0, idContact: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddContact")]
        public IActionResult AddContact([FromBody]OperationContactModel model)
        {
            var resultModel = new JsonModel();

            if(repo.AddContact(model.IdOperation, model.IdContact, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Contato adicionado com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Remove um contato de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idContact">id do contato</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}/Contact/{idContact}")]
        public IActionResult RemoveContact(int id, int idContact)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteContact(id, idContact, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Contato removido com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Adiciona um endereço a uma operação
        /// </summary>
        /// <param name="model">{ idOperation: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddAddress")]
        public IActionResult AddAddress([FromBody]OperationAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdOperation, model.IdAddress, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Endereço adicionado com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Remove um endereço de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}/Address/{idAddress}")]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteAddress(id, idAddress, out string error))
            {
                resultModel.Status = "ok";
                resultModel.Message = "Endereço removido com sucesso!";
            }
            else
            {
                resultModel.Status = "error";
                resultModel.Message = error;
            }

            return Ok(resultModel);
        }

        /// <summary>
        /// Lista as perguntas de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>lista das Perguntas da operação</returns>
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Faqs")]
        public IActionResult ListFaqs(int id)
        {
            var list = faqRepo.ListByOperation(id, out string error);

            var model = new JsonModel();
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<FaqModel>();
                list.ForEach(item => { ret.Add(new FaqModel(item)); });

                model.Status = "ok";
                model.Data = ret;
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
