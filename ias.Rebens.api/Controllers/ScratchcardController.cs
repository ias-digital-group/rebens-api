using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScratchcardController : BaseApiController
    {
        private IScratchcardDrawRepository drawRepo;
        private IScratchcardRepository repo;
        private IStaticTextRepository staticTextRepo;
        private IOperationRepository operationRepo;
        private IHostingEnvironment _hostingEnvironment;
        private ILogErrorRepository logErrorRepo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scratchcardRepository"></param>
        /// <param name="scratchcardPrizeRepository"></param>
        /// <param name="scratchcardDrawRepository"></param>
        /// <param name="hostingEnvironment"></param>
        public ScratchcardController(IScratchcardRepository scratchcardRepository, ILogErrorRepository logErrorRepository,
                                        IScratchcardDrawRepository scratchcardDrawRepository, IHostingEnvironment hostingEnvironment,
                                        IOperationRepository operationRepository, IStaticTextRepository staticTextRepository)
        {
            this.repo = scratchcardRepository;
            this.drawRepo = scratchcardDrawRepository;
            this.operationRepo = operationRepository;
            this._hostingEnvironment = hostingEnvironment;
            this.staticTextRepo = staticTextRepository;
            this.logErrorRepo = logErrorRepository;
        }

        /// <summary>
        /// Retorna uma lista de Campanhas de Raspadinhas
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Street, City, State), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">Id da operação, não obrigatório</param>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardListItemModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null, [FromQuery]int? status = null)
        {
            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation, status);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardListItemModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardListItemModel>()
                };
                foreach (var scratchcard in list.Page)
                    ret.Data.Add(new ScratchcardListItemModel(scratchcard));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma Campanha de raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha desejada</param>
        /// <returns>FAQ</returns>
        /// <response code="200">Retorna a raspadinha, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<ScratchcardModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var scratchcard = repo.Read(id, out bool canPublish, out string regulation, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (scratchcard == null || scratchcard.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<ScratchcardModel>() { Data = new ScratchcardModel(scratchcard)
                    {
                        CanPublish = canPublish,
                        ImagesPath = $"{Request.Scheme}://{Request.Host}/files/scratchcard/",
                        Regulation = regulation
                    }
                });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma Campanha de raspadinha
        /// </summary>
        /// <param name="scratchcard">Raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]ScratchcardModel scratchcard)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (scratchcard != null)
            {
                if (repo.Update(scratchcard.GetEntity(), idAdminUser, out string error))
                {
                    if (!string.IsNullOrEmpty(scratchcard.Regulation))
                    {
                        var reg = scratchcard.GetRegulation();
                        reg.Url = scratchcard.Id.ToString();
                        staticTextRepo.Update(reg, idAdminUser, out _);
                    }
                    return Ok(new JsonModel() { Status = "ok", Message = "Raspadinha atualizado com sucesso!" });
                }
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "O objeto não pode ser nulo!" });
        }

        /// <summary>
        /// Cria uma Campanha de raspadinha
        /// </summary>
        /// <param name="faq">Raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da raspadinha criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] ScratchcardModel scratchcard)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            int? idOperation = null;
            if (CheckRoles(new string[] { "administrator", "publisher"}))
            {
                idOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }


            if (scratchcard != null)
            {
                var s = scratchcard.GetEntity();
                s.IdAdminUser = idAdminUser;
                if (idOperation.HasValue)
                    s.IdOperation = idOperation.Value;
                if (repo.Create(s, idAdminUser, out string error))
                {
                    if(!string.IsNullOrEmpty(scratchcard.Regulation))
                    {
                        var reg = scratchcard.GetRegulation();
                        reg.Url = s.Id.ToString();
                        staticTextRepo.Create(reg, out _);
                    }
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Raspadinha criado com sucesso!", Id = s.Id });
                }

                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }
            return StatusCode(400, new JsonModel() { Status = "error", Message = "O objeto não pode ser nulo!" });
        }

        /// <summary>
        /// Apaga uma raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Raspadinha apagada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Recebe uma imagem e salva no servidor
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retorna a url da imagem, ou algum erro caso interno</response>
        /// <response code="204">Se ocorrer algum erro</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("UploadImage"), DisableRequestSizeLimit, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(FileUploadResultModel), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult UploadImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                string newPath = Path.Combine(_hostingEnvironment.WebRootPath, "files", "scratchcard");
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                if (file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString("n") + extension;
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new FileUploadResultModel() { FileName = fileName, Url = $"{Request.Scheme}://{Request.Host}/files/scratchcard/{fileName}" });
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(400, new JsonModel() { Status = "error", Message = ex.Message });
            }
        }

        /// <summary>
        /// Gera os bilhetes de uma campanha de raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Generate"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Generate(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            string newPath = Path.Combine(_hostingEnvironment.WebRootPath, "files", "scratchcard");
            repo.GenerateScratchcards(id, idAdminUser, newPath).ConfigureAwait(false);

            return Ok(new JsonModel() { Status = "ok", Message = "Os bilhetes da raspadinha estão sendo gerado!", Data = Enums.EnumHelper.GetEnumDescription(Enums.ScratchcardStatus.generating) });
        }

        /// <summary>
        /// Lista as operações que possuem o módulo de raspadinha habilitado
        /// </summary>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("Operations"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<List<OperationModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Operations()
        {
            var list = operationRepo.ListWithModule("coupon", out string error);
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
        /// Retorna uma lista de Raspadinhas de uma campanha
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="idScratchcard">Id da campanha de raspadinha, obrigatório</param>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}/Billets"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardDrawModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Billets(int id, [FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            var list = drawRepo.ListByScratchcard(id, page, pageItems, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardDrawModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardDrawModel>()
                };
                foreach (var scratchcard in list.Page)
                    ret.Data.Add(new ScratchcardDrawModel(scratchcard));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista de Raspadinhas de um cliente
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <returns>Lista com as raspadinahs encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ListByCustomer"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardDrawModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ListByCustomer([FromQuery]int page = 0, [FromQuery]int pageItems = 30)
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var list = drawRepo.ListByCustomer(idCustomer, page, pageItems, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardDrawModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardDrawModel>()
                };
                foreach (var scratchcard in list.Page)
                    ret.Data.Add(new ScratchcardDrawModel(scratchcard));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Marca raspadinha como aberta
        /// </summary>
        /// <param name="id">Id da raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SetOpened/{id}"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SetOpened(int id)
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (drawRepo.SetOpened(id, idCustomer))
                return Ok(new JsonModel() { Status = "ok" });

            return Ok(new JsonModel() { Status = "error", Message = "Cliente não reconhecido" });
        }

        /// <summary>
        /// Marca raspadinha como raspada
        /// </summary>
        /// <param name="id">Id da raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("SetPlayed/{id}"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult SetPlayed(int id)
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            int idOperation = GetOperationId(out errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if(drawRepo.SetPlayed(id, idCustomer, idOperation))
                return Ok(new JsonModel() { Status = "ok", Data = new { 
                    idStatus = (int)Enums.ScratchcardDraw.scratched,
                    status = Enums.EnumHelper.GetEnumDescription(Enums.ScratchcardDraw.scratched),
                    playedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Constant.TimeZone).ToString("dd/MM/yyyy", Constant.FormatProvider)
                } });

            return Ok(new JsonModel() { Status = "error", Message = "Cliente não reconhecido" });
        }

        /// <summary>
        /// Valida a raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("Validate/{id}"), Authorize("Bearer", Roles = "customer")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Validate(int id)
        {
            int idCustomer = GetCustomerId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (drawRepo.Validate(id, idCustomer))
                return Ok(new JsonModel() { Status = "ok" });

            return Ok(new JsonModel() { Status = "error", Message = "Cliente não reconhecido" });
        }

        /// <summary>
        /// Run job
        /// </summary>
        /// <returns>Retorna um objeto com o status (ok, error)</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("RunJob"), Authorize("Bearer", Roles = "master")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult RunJob()
        {
            try
            {
                logErrorRepo.Create("ScratchcardDailyJob", "START", "", "");
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "files", "scratchcard");

                var list = repo.ListByDistributionType(Enums.ScratchcardDistribution.daily);
                foreach (var item in list)
                {
                    var users = repo.ListCustomers(item.IdOperation, item.Type);
                    foreach (var user in users)
                    {
                        drawRepo.SaveRandom(item.Id, path, user, DateTime.Now.Date, (item.ScratchcardExpire ? (DateTime?)DateTime.Now.Date : null), out _);
                        Thread.Sleep(100);
                    }

                    Thread.Sleep(1000);
                }
                logErrorRepo.Create("ScratchcardDailyJob", "END", "", "");
            }
            catch( Exception ex)
            {
                return Ok(new JsonModel() { Status = "error", Message = ex.Message });
            }
            return Ok(new JsonModel() { Status = "ok" });
        }

        /// <summary>
        /// Ativa/Inativa uma campanha
        /// </summary>
        /// <param name="id">id da campanha</param>
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

            if (string.IsNullOrEmpty(error)) {
                return Ok(new JsonModel()
                {
                    Status = "ok",
                    Data = status,
                    Message = status ? Enums.EnumHelper.GetEnumDescription(Enums.ScratchcardStatus.active)
                                    : Enums.EnumHelper.GetEnumDescription(Enums.ScratchcardStatus.inactive)
                });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}