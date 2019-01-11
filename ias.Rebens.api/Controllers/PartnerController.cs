using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/Partner")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private IPartnerRepository repo;
        private IAddressRepository addressRepo;
        private IContactRepository contactRepo;
        private IBenefitRepository benefitRepo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="partnerRepository">Injeção de dependencia do repositório de parceiros</param>
        /// <param name="contactRepository">Injeção de dependencia do repositório de contato</param>
        /// <param name="addressRepository">Injeção de dependencia do repositório de endereço</param>
        /// <param name="benefitRepository">Injeção de dependencia do repositório de benefício</param>
        public PartnerController(IPartnerRepository partnerRepository, IContactRepository contactRepository, IAddressRepository addressRepository, IBenefitRepository benefitRepository)
        {
            this.repo = partnerRepository;
            this.addressRepo = addressRepository;
            this.contactRepo = contactRepository;
            this.benefitRepo = benefitRepository;
        }

        /// <summary>
        /// Lista todos os parceiros com paginação
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <returns>Lista com os parceiros encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<PartnerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListPartners([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Title ASC", [FromQuery]string searchWord = "")
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<PartnerModel>();
                ret.CurrentPage = list.CurrentPage;
                ret.HasNextPage = list.HasNextPage;
                ret.HasPreviousPage = list.HasPreviousPage;
                ret.ItemsPerPage = list.ItemsPerPage;
                ret.TotalItems = list.TotalItems;
                ret.TotalPages = list.TotalPages;
                ret.Data = new List<PartnerModel>();
                foreach (var part in list.Page)
                    ret.Data.Add(new PartnerModel(part));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna o parceiro conforme o ID
        /// </summary>
        /// <param name="id">Id do parceiro</param>
        /// <returns>Parceiros</returns>
        /// <response code="200">Retorna o parceiro, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<PartnerModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetPartner(int id)
        {
            var partner = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (partner == null || partner.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<PartnerModel>() { Data = new PartnerModel(partner) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um parceiro
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]PartnerModel partner)
        {
            var model = new JsonModel();
            string error = null;

            var part = partner.GetEntity();
            if (repo.Update(part, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Parceiro atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um parceiro
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do parceiro criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]PartnerModel partner)
        {
            string error = null;
            int idContact = 0;
            var model = new JsonModel();

            if (partner.Contact != null)
            {
                var contact = partner.Contact.GetEntity();

                if (partner.Contact.Address != null)
                {
                    var addr = partner.Contact.Address.GetEntity();
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
            

            var part = partner.GetEntity();
            if (repo.Create(part, out error))
            {
                if (idContact > 0)
                {
                    if (repo.AddContact(part.Id, idContact, out error))
                        return Ok(new JsonCreateResultModel() { Status = "ok", Message = "", Id = part.Id });

                    return StatusCode(400, new JsonModel() { Status = "error", Message = error });
                }
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "", Id = part.Id });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista os contatos de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
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
            var list = contactRepo.ListByPartner(id, out string error);

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
        /// Lista os endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
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
            var list = addressRepo.ListByPartner(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<AddressModel>>();
                ret.Data = new List<AddressModel>();
                foreach (var addr in list)
                    ret.Data.Add(new AddressModel(addr));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Adiciona um contato a um parceiro
        /// </summary>
        /// <param name="model">{ idPartner: 0, idContact: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um parceiro com um contato</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddContact")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddContact([FromBody]PartnerContactModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddContact(model.IdPartner, model.IdContact, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Contato adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um contato de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <param name="idContact">id do contato</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}/Contacts/{idContact}")]
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
        /// Adiciona um endereço a um parceiro
        /// </summary>
        /// <param name="model">{ idPartner: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Víncula um parceiro com um endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("AddAddress")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult AddAddress([FromBody]PartnerAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdPartner, model.IdAddress, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Endereço adicionado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Remove um endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="200">Remove o vínculo de benefício com endereço</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{Id}/Address/{idAddress}")]
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
        /// Lista os endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <returns>Lista com os benefícios encontradas</returns>
        /// <response code="200">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Benefits")]
        [ProducesResponseType(typeof(JsonDataModel<List<BenefitModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListBenefits(int id)
        {
            var list = benefitRepo.ListByPartner(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<BenefitModel>>();
                ret.Data = new List<BenefitModel>();
                foreach (var benfit in list)
                    ret.Data.Add(new BenefitModel(benfit));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}