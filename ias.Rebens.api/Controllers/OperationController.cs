using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
        private IStaticTextRepository staticTextRepo;
        private IBannerRepository bannerRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="operationRepository">Injeção de dependencia do repositório de operação</param>
        /// <param name="contactRepository">Injeção de dependencia do repositório de contato</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de endereço</param>
        /// <param name="faqRepository">Injeção de dependencia do repositório de faq</param>
        /// <param name="staticTextRepository">Injeção de dependencia do repositório de Texto</param>
        /// <param name="bannerRepository">Injeção de dependencia do repositório de Banner</param>
        public OperationController(IOperationRepository operationRepository, IContactRepository contactRepository, IAddressRepository addressRepository, IFaqRepository faqRepository, IStaticTextRepository staticTextRepository, IBannerRepository bannerRepository)
        {
            this.repo = operationRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;
            this.faqRepo = faqRepository;
            this.staticTextRepo = staticTextRepository;
            this.bannerRepo = bannerRepository;
        }

        /// <summary>
        /// Lista todas as operações com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Domain, Title, CompanyName, CompanyDoc), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com as operações encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<OperationModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
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
        [HttpGet("{id}")]
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
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]OperationModel operation)
        {
            var model = new JsonModel();

            var op = operation.GetEntity();
            if (repo.Update(op, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Operação atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da operação criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
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
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });

            
            if (repo.Create(op, out error))
            {
                if (idContact > 0)
                {
                    if (repo.AddContact(op.Id, idContact, out error))
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Operação criada com sucesso!", Id = op.Id });

                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                }
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Operação criada com sucesso!", Id = op.Id });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os contatos de uma operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Contacts")]
        [ProducesResponseType(typeof(JsonDataModel<List<ContactModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListContacts(int id)
        {
            var list = contactRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<ContactModel>>();
                ret.Data = new List<ContactModel>();
                foreach (var contact in list)
                    ret.Data.Add(new ContactModel(contact));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os endereço de uma Operação
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Address")]
        [ProducesResponseType(typeof(JsonDataModel<List<AddressModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListAddress(int id)
        {
            var list = addressRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<AddressModel>>();
                foreach (var addr in list)
                    ret.Data.Add(new AddressModel(addr));

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
        [HttpPost("AddContact")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddContact([FromBody]OperationContactModel model)
        {
            var resultModel = new JsonModel();

            if(repo.AddContact(model.IdOperation, model.IdContact, out string error))
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
        [HttpDelete("{id}/Contact/{idContact}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveContact(int id, int idContact)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteContact(id, idContact, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Contato removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um endereço a uma operação
        /// </summary>
        /// <param name="model">{ idOperation: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um parceiro com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddAddress")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress([FromBody]OperationAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdOperation, model.IdAddress, out string error))
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
        [HttpDelete("{id}/Address/{idAddress}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RemoveAddress(int id, int idAddress)
        {
            var resultModel = new JsonModel();

            if (repo.DeleteAddress(id, idAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço removido com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista as perguntas de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>lista das Perguntas da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Faqs")]
        [ProducesResponseType(typeof(JsonDataModel<List<FaqModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListFaqs(int id)
        {
            var list = faqRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<FaqModel>>();
                list.ForEach(item => { ret.Data.Add(new FaqModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os textos de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>lista dos Textos da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/StaticText")]
        [ProducesResponseType(typeof(JsonDataModel<List<StaticTextModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListStaticText(int id)
        {
            var list = staticTextRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<StaticTextModel>>();
                ret.Data = new List<StaticTextModel>();
                list.ForEach(item => { ret.Data.Add(new StaticTextModel(item)); });

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
        [HttpGet("{id}/StaticText/{idType}")]
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

        /// <summary>
        /// Lista os banners de uma operação 
        /// </summary>
        /// <param name="id">id da operação</param>
        /// <returns>lista dos banners da operação</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Banners")]
        [ProducesResponseType(typeof(JsonDataModel<List<BannerModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBanners(int id)
        {
            var list = bannerRepo.ListByOperation(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BannerModel>>();
                list.ForEach(item => { ret.Data.Add(new BannerModel(item)); });

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
