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
        /// <response code="201">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet]
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

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Retorna o parceiro conforme o ID
        /// </summary>
        /// <param name="id">Id do parceiro</param>
        /// <returns>Parceiros</returns>
        /// <response code="201">Retorna o parceiro, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}")]
        public IActionResult GetPartner(int id)
        {
            var partner = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (partner == null || partner.Id == 0)
                    return NoContent();
                return Ok(new { data = new PartnerModel(partner) });
            }

            var model = new JsonModel();
            model.Status = "error";
            model.Message = error;
            return Ok(model);
        }

        /// <summary>
        /// Atualiza um parceiro
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="201"></response>
        [HttpPut]
        public IActionResult Put([FromBody]PartnerModel partner)
        {
            var model = new JsonModel();
            string error = null;

            var part = partner.GetEntity();
            if (repo.Update(part, out error))
            {
                model.Status = "ok";
                model.Message = "Parceiro atualizado com sucesso!";
            }
            else
            {
                model.Status = "error";
                model.Message = error;
            }

            return Ok(model);
        }

        /// <summary>
        /// Cria um parceiro
        /// </summary>
        /// <param name="partner"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do parceiro criado</returns>
        /// <response code="201"></response>
        [HttpPost]
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
            {
                model.Status = "error";
                model.Message = error;
            }
            else
            {

                var part = partner.GetEntity();
                if (repo.Create(part, out error))
                {
                    if (idContact > 0)
                    {
                        if (repo.AddContact(part.Id, idContact, out error))
                        {
                            model.Status = "ok";
                            model.Message = "Parceiro criado com sucesso!";
                            model.Data = new { id = part.Id };
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
                        model.Message = "Parceiro criado com sucesso!";
                        model.Data = new { id = part.Id };
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
        /// Lista os contatos de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <returns>Lista com os contatos encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Contacts")]
        public IActionResult ListContacts(int id)
        {
            var list = contactRepo.ListByPartner(id, out string error);

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
        /// Lista os endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <returns>Lista com os endereços encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Address")]
        public IActionResult ListAddress(int id)
        {
            var list = addressRepo.ListByPartner(id, out string error);

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
        /// Adiciona um contato a um parceiro
        /// </summary>
        /// <param name="model">{ idPartner: 0, idContact: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddContact")]
        public IActionResult AddContact([FromBody]PartnerContactModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddContact(model.IdPartner, model.IdContact, out string error))
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
        /// Remove um contato de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <param name="idContact">id do contato</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpDelete("{id}/Contacts/{idContact}")]
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
        /// Adiciona um endereço a um parceiro
        /// </summary>
        /// <param name="model">{ idPartner: 0, idAddress: 0 }</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpPost("AddAddress")]
        public IActionResult AddAddress([FromBody]PartnerAddressModel model)
        {
            var resultModel = new JsonModel();

            if (repo.AddAddress(model.IdPartner, model.IdAddress, out string error))
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
        /// Remove um endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <param name="idAddress">id do endereço</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem.</returns>
        /// <response code="201"></response>
        [HttpDelete("{Id}/Address/{idAddress}")]
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
        /// Lista os endereço de um parceiro
        /// </summary>
        /// <param name="id">id do parceiro</param>
        /// <returns>Lista com os benefícios encontradas</returns>
        /// <response code="201">Retorna a list, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        [HttpGet("{id}/Benefits")]
        public IActionResult ListBenefits(int id)
        {
            var list = benefitRepo.ListByPartner(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new List<BenefitModel>();
                foreach (var benfit in list)
                    ret.Add(new BenefitModel(benfit));

                return Ok(new { data = ret });
            }

            var model = new JsonModel()
            {
                Status = "error",
                Message = error
            };
            return Ok(model);
        }
    }
}