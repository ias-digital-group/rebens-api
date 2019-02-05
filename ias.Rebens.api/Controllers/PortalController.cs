using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Portal Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PortalController : ControllerBase
    {
        private IBannerRepository bannerRepo;
        private IBenefitRepository benefitRepo;
        private IFaqRepository faqRepo;
        private IFormContactRepository formContactRepo;
        private IFormEstablishmentRepository formEstablishmentRepo;
        private IOperationRepository operationRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bannerRepository"></param>
        /// <param name="benefitRepository"></param>
        /// <param name="faqRepository"></param>
        /// <param name="formContactRepository"></param>
        /// <param name="operationRepository"></param>
        /// <param name="formEstablishmentRepository"></param>
        public PortalController(IBannerRepository bannerRepository, IBenefitRepository benefitRepository, IFaqRepository faqRepository, IFormContactRepository formContactRepository, IOperationRepository operationRepository, IFormEstablishmentRepository formEstablishmentRepository)
        {
            this.bannerRepo = bannerRepository;
            this.benefitRepo = benefitRepository;
            this.faqRepo = faqRepository;
            this.formContactRepo = formContactRepository;
            this.operationRepo = operationRepository;
            this.formEstablishmentRepo = formEstablishmentRepository;
        }

        /// <summary>
        /// Retorna os items da home não logada
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Retorna os items necessários para montar a home não logada</returns>
        /// <response code="200">Retorna o model com os items da home não logada, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("HomeLocked")]
        [ProducesResponseType(typeof(JsonDataModel<PortalHomeLockedModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult GetHomeLocked([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });


            var listFull = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Home, out string error);
            var listUnmissable = bannerRepo.ListByTypeAndOperation(operationGuid, (int)Enums.BannerType.Unmissable, out error);

            if (string.IsNullOrEmpty(error))
            {
                if ((listFull == null || listFull.Count == 0) && (listUnmissable == null || listUnmissable.Count == 0))
                    return NoContent();

                var ret = new JsonDataModel<PortalHomeLockedModel>()
                {
                    Data = new PortalHomeLockedModel()
                    {
                        BannerFullList = new List<PortalBannerModel>(),
                        BannerUnmissable = new List<PortalBannerModel>()
                    }
                };
                foreach (var banner in listUnmissable)
                    ret.Data.BannerUnmissable.Add(new PortalBannerModel(banner, null, null));

                foreach (var banner in listFull)
                {
                    string call = null, logo = null;
                    if (banner.IdBenefit.HasValue)
                        benefitRepo.ReadCallAndPartnerLogo(banner.IdBenefit.Value, out call, out logo, out error);
                    ret.Data.BannerFullList.Add(new PortalBannerModel(banner, call, logo));
                }

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista com as perguntas frequentes
        /// </summary>
        /// <param name="operationCode">código da operação</param>
        /// <returns>Lista das perguntas frequentes</returns>
        /// <response code="200">Retorna a lista das perguntas frequentes, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListFaq")]
        [ProducesResponseType(typeof(JsonDataModel<List<FaqModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListFaq([FromHeader(Name = "x-operation-code")]string operationCode)
        {
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });


            var list = faqRepo.ListByOperation(operationGuid, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<FaqModel>>()
                {
                    Data = new List<FaqModel>()
                };

                foreach (var item in list)
                    ret.Data.Add(new FaqModel(item));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salvar um contato
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="formContact"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("ContactForm")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ContactForm([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody] FormContactModel formContact)
        {
            var model = new JsonModel();
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var f = formContact.GetEntity();
                f.IdOperation = operation.Id;
                if (formContactRepo.Create(f, out error))
                {
                    //var sendingBlue = new Integration.SendinBlueHelper();
                    //sendingBlue.Send(email.ToEmail, email.ToName, email.FromEmail, email.FromName, email.Subject, email.Message);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Contato enviado com sucesso!", Id = f.Id });
                }
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Salvar um formulário de indicação de benefícios
        /// </summary>
        /// <param name="operationCode"></param>
        /// <param name="formEstablishment"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da faq criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("EstablishmentForm")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult EstablishmentForm([FromHeader(Name = "x-operation-code")]string operationCode, [FromBody] FormEstablishmentModel formEstablishment)
        {
            var model = new JsonModel();
            Guid operationGuid = Guid.Empty;
            Guid.TryParse(operationCode, out operationGuid);

            if (operationGuid == Guid.Empty)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            var operation = operationRepo.Read(operationGuid, out string error);

            if (operation != null)
            {
                var f = formEstablishment.GetEntity();
                f.IdOperation = operation.Id;
                if (formEstablishmentRepo.Create(f, out error))
                {
                    //var sendingBlue = new Integration.SendinBlueHelper();
                    //sendingBlue.Send(email.ToEmail, email.ToName, email.FromEmail, email.FromName, email.Subject, email.Message);

                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Indicação enviada com sucesso!", Id = f.Id });
                }
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não reconhecida!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}