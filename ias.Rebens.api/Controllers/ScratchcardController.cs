using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,publiser,administratorRebens,publisherRebens")]
    [ApiController]
    public class ScratchcardController : ControllerBase
    {
        private IScratchcardDrawRepository drawRepo;
        private IScratchcardPrizeRepository prizeRepo;
        private IScratchcardRepository repo;
        private IOperationRepository operationRepo;
        private IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scratchcardRepository"></param>
        /// <param name="scratchcardPrizeRepository"></param>
        /// <param name="scratchcardDrawRepository"></param>
        /// <param name="hostingEnvironment"></param>
        public ScratchcardController(IScratchcardRepository scratchcardRepository, IScratchcardPrizeRepository scratchcardPrizeRepository,
                                        IScratchcardDrawRepository scratchcardDrawRepository, IHostingEnvironment hostingEnvironment,
                                        IOperationRepository operationRepository)
        {
            this.repo = scratchcardRepository;
            this.prizeRepo = scratchcardPrizeRepository;
            this.drawRepo = scratchcardDrawRepository;
            this.operationRepo = operationRepository;
            this._hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Retorna uma lista de Raspadinhas
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
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<ScratchcardModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 30, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "", [FromQuery]int? idOperation = null)
        {
            var principal = HttpContext.User;
            if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
            {
                if (principal?.Claims != null)
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            var list = repo.ListPage(page, pageItems, searchWord, sort, out string error, idOperation);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<ScratchcardModel>
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<ScratchcardModel>()
                };
                foreach (var scratchcard in list.Page)
                {
                    var operationName = operationRepo.GetName(scratchcard.IdOperation, out _);
                    ret.Data.Add(new ScratchcardModel(scratchcard, operationName));
                }

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha desejada</param>
        /// <returns>FAQ</returns>
        /// <response code="200">Retorna a raspadinha, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<ScratchcardModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var scratchcard = repo.Read(id, out bool canPublish, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (scratchcard == null || scratchcard.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<ScratchcardModel>() { Data = new ScratchcardModel(scratchcard)
                {
                    CanPublish = canPublish
                }
                });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza uma raspadinha
        /// </summary>
        /// <param name="scratchcard">Raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]ScratchcardModel scratchcard)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
                if (int.TryParse(userId.Value, out int tmpId))
                    idAdminUser = tmpId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            if (scratchcard != null)
            {
                if (repo.Update(scratchcard.GetEntity(), idAdminUser, out string error))
                    return Ok(new JsonModel() { Status = "ok", Message = "Raspadinha atualizado com sucesso!" });
                return StatusCode(400, new JsonModel() { Status = "error", Message = error });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "O objeto não pode ser nulo!" });
        }

        /// <summary>
        /// Cria uma raspadinha
        /// </summary>
        /// <param name="faq">Raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id da raspadinha criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]ScratchcardModel scratchcard)
        {
            int idAdminUser;
            int? idOperation = null;
            var principal = HttpContext.User;

            if (principal?.Claims != null)
            {
                var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
                if (int.TryParse(userId.Value, out int idUser))
                    idAdminUser = idUser;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });


                if (principal.IsInRole("administrator") || principal.IsInRole("publisher"))
                {
                    var operationId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                    if (operationId == null)
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                    if (int.TryParse(operationId.Value, out int tmpId))
                        idOperation = tmpId;
                    else
                        return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
                }
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });


            if (scratchcard != null)
            {
                var s = scratchcard.GetEntity();
                if (idOperation.HasValue)
                    s.IdOperation = idOperation.Value;
                if (repo.Create(s, idAdminUser, out string error))
                    return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Raspadinha criado com sucesso!", Id = s.Id });

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
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
                if (int.TryParse(userId.Value, out int tmpId))
                    idAdminUser = tmpId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

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
        [HttpPost("UploadImage"), DisableRequestSizeLimit]
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
        /// Gera os bilhetes de uma raspadinha
        /// </summary>
        /// <param name="id">Id da raspadinha</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se a geração iniciar com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost("{id}/Generate")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Generate(int id)
        {
            int idAdminUser;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var userId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userId == null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
                if (int.TryParse(userId.Value, out int tmpId))
                    idAdminUser = tmpId;
                else
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            }
            else
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });

            string newPath = Path.Combine(_hostingEnvironment.WebRootPath, "files", "scratchcard");
            repo.GenerateScratchcards(id, idAdminUser, newPath).ConfigureAwait(false);

            return Ok(new JsonModel() { Status = "ok", Message = "Os bilhetes da raspadinha estão sendo gerado!", Data = Enums.EnumHelper.GetEnumDescription(Enums.ScratchcardStatus.generating) });
        }
    }
}