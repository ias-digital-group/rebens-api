using System;
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
    public class OperationController : BaseApiController
    {
        private IOperationRepository repo;
        private IAddressRepository addressRepo;
        private ILogErrorRepository logError;
        private IContactRepository contactRepo;
        private IFaqRepository faqRepo;
        private IStaticTextRepository staticTextRepo;
        private IBannerRepository bannerRepo;
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
        /// <param name="hostingEnvironment">Injeção de dependencia do repositório de Clientes da operação</param>
        /// <param name="logger">Injeção de dependencia do repositório de Clientes da operação</param>
        /// <param name="logError">Injeção de dependencia do repositório de Clientes da operação</param>
        public OperationController(IOperationRepository operationRepository, IContactRepository contactRepository, IAddressRepository addressRepository, 
            IFaqRepository faqRepository, IStaticTextRepository staticTextRepository, IBannerRepository bannerRepository,
            IHostingEnvironment hostingEnvironment, ILogger<OperationController> logger,
            ILogErrorRepository logError, IFileToProcessRepository fileToProcessRepository, IModuleRepository moduleRepository)
        {
            this.repo = operationRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;
            this.faqRepo = faqRepository;
            this.staticTextRepo = staticTextRepository;
            this.bannerRepo = bannerRepository;
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
        [HttpGet, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens,publisher,administrator,promoter")]
        [ProducesResponseType(typeof(ResultPageModel<OperationListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "", [FromQuery]bool? active = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, active);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<OperationListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<OperationListItemModel>()
                };
                foreach (var operation in list.Page)
                    ret.Data.Add(new OperationListItemModel(operation));

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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if(operation == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "objeto nulo"});

            var op = operation.GetEntity();
            if (operation.MainAddress != null)
            {
                if (operation.MainAddress.Id > 0)
                    addressRepo.Update(operation.MainAddress.GetEntity(), idAdminUser, out _);
                else
                {
                    var addr = operation.MainAddress.GetEntity();
                    if (addressRepo.Create(addr, idAdminUser, out _))
                        op.IdMainAddress = addr.Id;
                }
            }
            if (operation.MainContact != null)
            {
                if (operation.MainContact.Id > 0)
                    contactRepo.Update(operation.MainContact.GetEntity(), idAdminUser, out _);
                else
                {
                    var contact = operation.MainContact.GetEntity();
                    if (contactRepo.Create(contact, idAdminUser, out _))
                        op.IdMainContact = contact.Id;
                }
            }

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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (operation == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "objeto nulo" });

            var op = operation.GetEntity();
            string error;
            if (operation.MainContact != null)
            {
                var contact = operation.MainContact.GetEntity();
                if (contactRepo.Create(contact, idAdminUser, out error))
                    op.IdMainContact = contact.Id;
            }
            if (operation.MainAddress != null)
            {
                var addr = operation.MainAddress.GetEntity();
                if (addressRepo.Create(addr, idAdminUser, out error))
                    op.IdMainAddress = addr.Id;
            }

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

                if (operation.Contact != null)
                {
                    var contact = operation.Contact.GetEntity();
                    contact.Type = (int)Enums.LogItem.Operation;
                    contact.Deleted = false;
                    contact.IdItem = op.Id;
                    if (operation.Contact.Address != null)
                    {
                        var addr = operation.Contact.Address.GetEntity();
                        if (addressRepo.Create(addr, idAdminUser, out _))
                            contact.IdAddress = addr.Id;
                    }

                    contactRepo.Create(contact, idAdminUser, out _);
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
            var operation = repo.Read(id, out string error);
            if(operation == null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            else if(operation.IsCustom)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não pode ser publicada!" });

            if (repo.ValidateOperation(id, out error))
            {
                var ret = repo.GetPublishData(id, out string domain, out error);
                if (ret != null)
                {
                    int idAdminUser = GetAdminUserId(out string errorId);
                    if (errorId != null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

                    repo.SavePublishStatus(id, (int)Enums.PublishStatus.processing, idAdminUser, null, out _);

                    try
                    {
                        if (domain.Contains(".sistemarebens."))
                        {
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
                        catch(Exception ex)
                        {
                            repo.SavePublishStatus(id, (int)Enums.PublishStatus.error, idAdminUser, null, out error);
                            logError.Create("OperationController.Publish - builder", ex.Message, "", ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        int idError = logError.Create("OperationController.Publish", ex.Message, "", ex.StackTrace);
                        repo.SavePublishStatus(id, (int)Enums.PublishStatus.error, idAdminUser, idError, out error);
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
            if (Guid.TryParse(code, out Guid operationGuid))
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

        /// <summary>
        /// Lista todas as operações ativas que possuem o módulo habilitado
        /// </summary>
        /// <param name="module">Módulo que deve estar habilitado para operação</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListByModule/{module}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<List<OperationModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListByModule(string module)
        {
            var list = repo.ListWithModule(module, out string error);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<OperationModel>>()
                {
                    Data = new List<OperationModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new OperationModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        
        /// <summary>
        /// Ativa/Inativa a operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns></returns>
        /// <response code="200">Se o tudo ocorrer sem erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/ToggleActive")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ToggleActive(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var status = repo.ToggleActive(id, idAdminUser, out string error);

            if (string.IsNullOrEmpty(error))
                return Ok(new JsonModel() { Status = "ok", Data = status });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
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
            StaticText config;
            string error;
            if (id == 0)
                config = staticTextRepo.ReadByType((int)Enums.StaticTextType.OperationConfigurationDefault, out error);
            else
                config = staticTextRepo.ReadOperationConfiguration(id, out error);

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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

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

            if (staticTextRepo.Update(config, idAdminUser, out string error))
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

                var ret = new ResultPageModel<ContactModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ContactModel>()
                };
                foreach (var contact in list.Page)
                    ret.Data.Add(new ContactModel(contact));

                return Ok(ret);
            }

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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

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
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

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

                var ret = new ResultPageModel<FaqModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<FaqModel>()
                };
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

                var ret = new ResultPageModel<StaticTextModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<StaticTextModel>()
                };
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

                var ret = new ResultPageModel<BannerModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<BannerModel>()
                };
                foreach (var banner in list.Page)
                    ret.Data.Add(new BannerModel(banner));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
        #endregion Banner

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
            var list = moduleRepo.ListActive(out string error);
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