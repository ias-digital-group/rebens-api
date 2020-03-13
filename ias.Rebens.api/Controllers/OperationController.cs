﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Linq;
using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Operation Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/Operation")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private IOperationRepository repo;
        private IAddressRepository addressRepo;
        private ILogErrorRepository logError;
        private IContactRepository contactRepo;
        private IFaqRepository faqRepo;
        private IStaticTextRepository staticTextRepo;
        private IBannerRepository bannerRepo;
        private IOperationCustomerRepository operationCustomerRepo;
        private IFileToProcessRepository fileToProcessRepo;
        private IHostingEnvironment _hostingEnvironment;
        private IModuleRepository moduleRepo;
        private ILogger<OperationController> _logger;
        private Constant constant;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="operationRepository">Injeção de dependencia do repositório de operação</param>
        /// <param name="contactRepository">Injeção de dependencia do repositório de contato</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de endereço</param>
        /// <param name="faqRepository">Injeção de dependencia do repositório de faq</param>
        /// <param name="staticTextRepository">Injeção de dependencia do repositório de Texto</param>
        /// <param name="bannerRepository">Injeção de dependencia do repositório de Banner</param>
        /// <param name="operationCustomerRepository">Injeção de dependencia do repositório de Clientes da operação</param>
        /// <param name="hostingEnvironment">Injeção de dependencia do repositório de Clientes da operação</param>
        /// <param name="logger">Injeção de dependencia do repositório de Clientes da operação</param>
        /// <param name="logError">Injeção de dependencia do repositório de Clientes da operação</param>
        public OperationController(IOperationRepository operationRepository, IContactRepository contactRepository, IAddressRepository addressRepository, 
            IFaqRepository faqRepository, IStaticTextRepository staticTextRepository, IBannerRepository bannerRepository,
            IOperationCustomerRepository operationCustomerRepository, IHostingEnvironment hostingEnvironment, ILogger<OperationController> logger,
            ILogErrorRepository logError, IFileToProcessRepository fileToProcessRepository, IModuleRepository moduleRepository)
        {
            this.repo = operationRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;
            this.faqRepo = faqRepository;
            this.staticTextRepo = staticTextRepository;
            this.bannerRepo = bannerRepository;
            this.operationCustomerRepo = operationCustomerRepository;
            this._hostingEnvironment = hostingEnvironment;
            this._logger = logger;
            this.logError = logError;
            this.fileToProcessRepo = fileToProcessRepository;
            this.moduleRepo = moduleRepository;
            this.constant = new Constant();
        }

        #region Operation
        /// <summary>
        /// Lista todas as operações com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Domain, Title, CompanyName, CompanyDoc), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<OperationModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]bool? active = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, active);

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

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a operação conforme o ID
        /// </summary>
        /// <param name="id">Id da operação desejada</param>
        /// <returns>Operação</returns>
        /// <response code="200">Retorna a operação, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<OperationModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var operation = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (operation == null || operation.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<OperationModel>() { Data = new OperationModel(operation) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "master")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]OperationModel operation)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

            var op = operation.GetEntity();
            if (repo.Update(op, idAdminUser, out string error))
            {
                repo.ValidateOperation(op.Id, out error);
                return Ok(new JsonModel() { Status = "ok", Message = "Operação atualizada com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id e o Código da operação criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] OperationModel operation)
        {
            string error = null;
            int idContact = 0;
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

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
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            
            if (repo.Create(op, idAdminUser, out error))
            {
                var sendingblue = new Integration.SendinBlueHelper();
                if(sendingblue.CreateList(op.Title, out int listId, out string error1))
                {
                    if(!repo.SaveSendingblueListId(op.Id, listId, out error1))
                    {
                        logError.Create("OperationController.Post", error1, "Save Sending blue list id", $"listId: {listId}");
                    }
                }
                else
                {
                    logError.Create("OperationController.Post", error1, "Create Sending blue list id", "");
                }

                if (idContact > 0)
                {
                    if (repo.AddContact(op.Id, idContact, idAdminUser, out error))
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Operação criada com sucesso!", Id = op.Id, Extra = op.Code.ToString() });

                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                }
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Operação criada com sucesso!", Id = op.Id, Extra = op.Code.ToString() });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Coloca uma operação na fila de publicação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns></returns>
        /// <response code="200">Opreação em fila</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/publish"), Authorize("Bearer", Roles = "master")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Publish(int id)
        {
            if (repo.ValidateOperation(id, out string error))
            {
                var ret = repo.GetPublishData(id, out string domain, out error);
                if (ret != null)
                {
                    int idAdminUser;
                    var principal = HttpContext.User;
                    if (principal?.Claims != null)
                    {
                        var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                        if (tmpId == null)
                            return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                        if (int.TryParse(tmpId.Value, out int usrId))
                            idAdminUser = usrId;
                        else
                            return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                    }
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

                    repo.SavePublishStatus(id, (int)Enums.PublishStatus.processing, idAdminUser, null, out _);

                    if (domain.Contains(".sistemarebens."))
                    {
                        var operation = repo.Read(id, out error);
                        if (!operation.SubdomainCreated)
                        {
                            var awsHelper = new Integration.AWSHelper();
                            awsHelper.AddDomainToRoute53(operation.TemporarySubdomain, id, this.repo);
                        }
                    }

                    string content = JsonConvert.SerializeObject(ret);
                    HttpWebRequest request = WebRequest.Create(new Uri($"{constant.BuilderUrl}api/operations")) as HttpWebRequest;

                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Timeout = 30000;

                    using (Stream s = request.GetRequestStream())
                    {
                        using (StreamWriter sw = new StreamWriter(s))
                            sw.Write(content);
                    }
                    try
                    {
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        if (response.StatusCode != HttpStatusCode.OK)
                            repo.SavePublishStatus(id, (int)Enums.PublishStatus.error, idAdminUser, null, out error);

                        return StatusCode(200, new JsonModel() { Status = "ok", Data = ret });
                    }
                    catch
                    {
                        repo.SavePublishStatus(id, (int)Enums.PublishStatus.error, idAdminUser, null, out error);
                    }
                }
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpGet("BuilderDone/{code}"), AllowAnonymous]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult BuilderDone(string code)
        {
            Guid operationGuid = Guid.Empty;
            if (Guid.TryParse(code, out operationGuid))
            {
                if (operationGuid != Guid.Empty)
                {
                    if (repo.SavePublishDone(operationGuid, out string error))
                    {
                        return Ok(new JsonModel() { Status = "ok" });
                    }

                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = "GUID empty" });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "GUID parse error" });
        }
        #endregion Operation

        #region Operation Configuration
        /// <summary>
        /// Retorna o objeto com as configurações da operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>Retorna o objeto com as configurações da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Configuration"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<object>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetConfiguration(int id)
        {
            var config = staticTextRepo.ReadOperationConfiguration(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (config == null || config.Id == 0)
                    return NoContent();

                var ret = new JsonDataModel<object>() { Data = JObject.Parse(config.Html) };

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salva as configurações de publicação da Operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="data">configurações</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Configurações salva com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Configuration"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SaveConfiguration(int id, [FromBody]object data)
        {
            var serializedData = JsonConvert.SerializeObject(data);

            var config = new StaticText()
            {
                Title = "Configuração de Operação - " + id,
                Url = "operation-" + id,
                Html = serializedData,
                Style = "",
                Order = 0,
                IdStaticTextType = (int)Enums.StaticTextType.OperationConfiguration,
                IdOperation = id,
                IdBenefit = null,
                Active = true,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            if (staticTextRepo.Update(config, out string error))
            {
                repo.ValidateOperation(id, out _);
                return Ok(new JsonModel() { Status = "ok", Message = "Configuração salva com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Operation Configuration

        #region OperationContacts
        /// <summary>
        /// Lista os contatos de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Email, JobTitle), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Contacts"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<ContactModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListContacts(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = contactRepo.ListByOperation(id, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ContactModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<ContactModel>();
                foreach (var contact in list.Page)
                    ret.Data.Add(new ContactModel(contact));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um contato a uma operação
        /// </summary>
        /// <param name="model">{ idOperation: 0, idContact: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula uma operação com um contato</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddContact"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddContact([FromBody]OperationContactModel model)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

            if (repo.AddContact(model.IdOperation, model.IdContact, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Contato adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um contato de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idContact">id do contato</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Contact/{idContact}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveContact(int id, int idContact)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

            if (repo.DeleteContact(id, idContact, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Contato removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion OperationContacts

        #region OperationAddress
        /// <summary>
        /// Lista os endereço de uma Operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Address"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<AddressModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListAddress(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = addressRepo.ListByOperation(id, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<AddressModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<AddressModel>();
                foreach (var addr in list.Page)
                    ret.Data.Add(new AddressModel(addr));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um endereço a uma operação
        /// </summary>
        /// <param name="model">{ idOperation: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um parceiro com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddAddress"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress([FromBody]OperationAddressModel model)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

            if (repo.AddAddress(model.IdOperation, model.IdAddress, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um endereço de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Address/{idAddress}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tmpId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tmpId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
                if (int.TryParse(tmpId.Value, out int usrId))
                    idAdminUser = usrId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Sem permissão de acesso!" });

            if (repo.DeleteAddress(id, idAddress, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion OperationAddress

        #region Faq
        /// <summary>
        /// Lista as perguntas de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Question, Answer, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>lista das Perguntas da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Faqs"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<FaqModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListFaqs(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = faqRepo.ListByOperation(id, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
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

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Faq

        #region StaticText
        /// <summary>
        /// Lista os textos de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Title, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>lista dos Textos da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/StaticText"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<StaticTextModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListStaticText(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = staticTextRepo.ListByOperation(id, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<StaticTextModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<StaticTextModel>();
                foreach (var staticText in list.Page)
                    ret.Data.Add(new StaticTextModel(staticText));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna um texto pelo tipo e operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idType">id do tipo de texto</param>
        /// <returns>Texto</returns>
        /// <response code="200">Retorna o Texto, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/StaticText/{idType}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<StaticTextModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ReadStaticText(int id, int idType)
        {
            var text = staticTextRepo.ReadByType(id, idType, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (text == null)
                    return NoContent();

                var ret = new JsonDataModel<StaticTextModel>();
                ret.Data = new StaticTextModel(text); 

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion StaticText

        #region Banner
        /// <summary>
        /// Lista os banners de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>lista dos banners da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Banners"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<BannerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBanners(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = bannerRepo.ListByOperation(id, page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<BannerModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<BannerModel>();
                foreach (var banner in list.Page)
                    ret.Data.Add(new BannerModel(banner));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Banner

        #region Operation Customers
        /// <summary>
        /// Lista os clientes pré cadastrados de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>lista dos banners da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Customers"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher")]
        [ProducesResponseType(typeof(ResultPageModel<OperationCustomerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListCustomers(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "")
        {
            var list = operationCustomerRepo.ListPage(page, pageItems, searchWord, sort, out string error, id);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<OperationCustomerModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<OperationCustomerModel>()
                };
                
                foreach (var customer in list.Page)
                    ret.Data.Add(new OperationCustomerModel(customer));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um cliente pré cadastrado
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <param name="idCustomer">id do cliente</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Cliente foi apagado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Customers/{idCustomer}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult DeleteCustomer(int id, int idCustomer)
        {
            var resultModel = new JsonModel();

            if (operationCustomerRepo.Delete(idCustomer, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Cliente apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um cliente pré
        /// </summary>
        /// <param name="id">Id da operação</param>
        /// <param name="customer"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Customers/"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CreateCustomer(int id, [FromBody] OperationCustomerModel customer)
        {
            var c = customer.GetEntity();
            c.IdOperation = id;
            c.Signed = false;
            if (operationCustomerRepo.Create(c, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Cliente criado com sucesso!", Id = c.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma list de cliente pré, a partir de um arquivo excel
        /// </summary>
        /// <param name="id">Id da operação</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/UploadCustomersList"), DisableRequestSizeLimit, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult CreateListOfCustomers(int id)
        {
            try
            {
                var file = Request.Form.Files[0];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, "files");
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    var list = new List<OperationCustomer>();
                    ISheet sheet;
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString("n") + extension;
                    string fullPath = Path.Combine(newPath, fileName);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var fileToProcess = new FileToProcess()
                    {
                        Name = fullPath,
                        Created = DateTime.UtcNow,
                        IdOperation = id,
                        Modified = DateTime.UtcNow,
                        Total = 0,
                        Processed = 0,
                        Status = (int)Enums.FileToProcessStatus.New,
                        Type = (int)Enums.FileToProcessType.OperationCustomer
                    };

                    this.fileToProcessRepo.Create(fileToProcess, out _);

                    try
                    {
                        using (var stream = new StreamReader(fullPath))
                        {
                            if (extension == ".xls")
                            {
                                HSSFWorkbook hssfwb = new HSSFWorkbook(stream.BaseStream); //This will read the Excel 97-2000 formats  
                                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                            }
                            else
                            {
                                XSSFWorkbook hssfwb = new XSSFWorkbook(stream.BaseStream); //This will read 2007 Excel format  
                                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                            }
                            int total = sheet.LastRowNum;
                            this.fileToProcessRepo.UpdateTotal(fileToProcess.Id, total, out _);

                            IRow headerRow = sheet.GetRow(0); //Get Header Row
                            int cellCount = headerRow.LastCellNum;
                            bool isValid = true;

                            ICell cellName = headerRow.GetCell(0);
                            ICell cellCPf = headerRow.GetCell(1);
                            ICell cellPhone = headerRow.GetCell(2);
                            ICell cellCellphone = headerRow.GetCell(3);
                            ICell cellEmail1 = headerRow.GetCell(4);
                            ICell cellEmail2 = headerRow.GetCell(5);

                            isValid = cellName != null && cellName.ToString().Trim().ToLower() == "nome" &&
                                cellCPf != null && cellCPf.ToString().Trim().ToLower() == "cpf" &&
                                cellPhone != null && cellPhone.ToString().Trim().ToLower() == "telefone" &&
                                cellCellphone != null && cellCellphone.ToString().Trim().ToLower() == "celular" &&
                                cellEmail1 != null && cellEmail1.ToString().Trim().ToLower() == "email1" &&
                                cellEmail2 != null && cellEmail2.ToString().Trim().ToLower() == "email2";

                            if (!isValid)
                            {
                                this.fileToProcessRepo.UpdateStatus(fileToProcess.Id, (int)Enums.FileToProcessStatus.Error, out _);
                                return Ok(new JsonModel() { Status = "error", Message = "O arquivo não está no formato correto!" });
                            }
                            else
                            {
                                this.fileToProcessRepo.UpdateStatus(fileToProcess.Id, (int)Enums.FileToProcessStatus.Ready, out _);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
                    }

                    //int counter = 0;
                    //foreach(var customer in list)
                    //{
                    //    if (operationCustomerRepo.Create(customer, out string error))
                    //        counter++;
                    //}

                    return Ok(new JsonModel() { Status = "ok", Message = $"O arquivo será processado em breve." });
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se existe algum arquivo em processamento
        /// </summary>
        /// <param name="id">Id da operação</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/HasFileProcessing/"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,administrator,publisher")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult HasFileProcessing(int id)
        {
            var ret = fileToProcessRepo.ReadByType((int)Enums.FileToProcessType.OperationCustomer, (int)Enums.FileToProcessStatus.Processing, id, out _);
            if (ret != null)
            {
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = $"Processando ({ret.Processed}/{ret.Total})" });
            } 
            else
            {
                ret = fileToProcessRepo.ReadByType((int)Enums.FileToProcessType.OperationCustomer, (int)Enums.FileToProcessStatus.New, id, out _);
                if (ret != null)
                {
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = $"O arquivo está na fila para ser processado!" });
                }
                else
                {
                    ret = fileToProcessRepo.ReadByType((int)Enums.FileToProcessType.OperationCustomer, (int)Enums.FileToProcessStatus.Ready, id, out _);
                    if (ret != null)
                    {
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = $"O arquivo está na fila para ser processado!" });
                    }
                }
            }

            return NoContent();
        }
        #endregion Operation Customers

        #region Modules
        /// <summary>
        /// Lista os módulos
        /// </summary>
        /// <returns>lista dos módulos</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Modules/{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(List<ModuleModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListModules(int id)
        {
            var list = moduleRepo.List(out string error);
            List<ModuleModel> modules = null;
            var ret = new List<ModuleModel>();

            if (id > 0)
            {
                var configuration = staticTextRepo.ReadOperationConfiguration(id, out _);
                var config = Helper.Config.JsonHelper<Helper.Config.OperationConfiguration>.GetObject(configuration.Html);
                modules = config.Modules;
            }
            
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                foreach (var m in list)
                {
                    var item = new ModuleModel(m);
                    if(modules != null && modules.Any(md => md.Name == m.Name))
                    {
                        item = modules.Single(md => md.Name == m.Name);   
                    }
                    ret.Add(item);
                }   

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Modules
    }
}