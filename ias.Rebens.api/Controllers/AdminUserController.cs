using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.Streaming.Values;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// AdminUser Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administrator,administratorRebens,partnerAdministrator,publisher,publisherRebens,promoter,ticketChecker,couponChecker,partnerApprover")]
    [ApiController]
    public class AdminUserController : BaseApiController
    {
        private IAdminUserRepository repo;
        private Constant constant;

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="adminUserRepository"></param>
        public AdminUserController(IAdminUserRepository adminUserRepository)
        {
            this.repo = adminUserRepository;
            this.constant = new Constant();
        }

        /// <summary>
        /// Retorna uma lista de usuários conforme os parametros
        /// </summary>
        /// <param name="page">página, não obrigatório (default=0)</param>
        /// <param name="pageItems">itens por página, não obrigatório (default=30)</param>
        /// <param name="sort">Ordenação campos (Id, Name, Order), direção (ASC, DESC)</param>
        /// <param name="searchWord">Palavra à ser buscada</param>
        /// <param name="idOperation">id da operação, não obrigatório (default=null)</param>
        /// <param name="active">active, não obrigatório (default=null)</param>
        /// <param name="role">papel, não obrigatório (default=null)</param>
        /// <param name="idOperationPartner">id do parceiro, não obrigatório (default=null)</param>
        /// <returns>Lista com os usuários encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResultPageModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int page = 0, [FromQuery]int pageItems = 25, [FromQuery]string sort = "Name ASC", [FromQuery]string searchWord = "",
            [FromQuery]int? idOperation = null, [FromQuery]bool? active = null, [FromQuery]string role = null, [FromQuery]int? idOperationPartner = null)
        {
            if (CheckRoles(new string[2] { "administrator", "partnerAdministrator" }))
            {
                idOperation = GetOperationId(out string errorId);
                if(errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Operação não encontrada!" });
            }

            idOperationPartner = null;
            if (CheckRole("partnerAdministrator"))
            {
                idOperationPartner = GetOperationPartnerId(out string errorId);
                if(errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Parceiro não encontrada!" });
            }

            string userRole = GetRole(out string roleError);
            if (roleError != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = roleError });

            var list = repo.ListPage(userRole, page, pageItems, searchWord, sort, out string error, idOperation, active, role, idOperationPartner);
            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.TotalItems == 0)
                    return NoContent();

                var ret = new ResultPageModel<AdminUserModel>()
                {
                    CurrentPage = list.CurrentPage,
                    HasNextPage = list.HasNextPage,
                    HasPreviousPage = list.HasPreviousPage,
                    ItemsPerPage = list.ItemsPerPage,
                    TotalItems = list.TotalItems,
                    TotalPages = list.TotalPages,
                    Data = new List<AdminUserModel>()
                };
                foreach (var admin in list.Page)
                    ret.Data.Add(new AdminUserModel(admin));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna Usuário conforme o ID
        /// </summary>
        /// <param name="id">Id do usuário desejada</param>
        /// <returns>Categoria</returns>
        /// <response code="200">Retorna o usuário, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<AdminUserModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            if (CheckRoles(new string[3] { "publisher", "publisherRebens", "promoter" }))
            {
                id = GetAdminUserId(out string errorId);
                if(errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
            }

            var admin = repo.ReadFull(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (admin == null || admin.Id == 0)
                    return NoContent();
                return Ok(new JsonDataModel<AdminUserModel>() { Data = new AdminUserModel(admin) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]AdminUserModel user)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if(errorId !=null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (CheckRoles(new string[] { "publisher", "publisherRebens", "promoter" }) && user.Id != idAdminUser)
                return StatusCode(400, new JsonModel() { Status = "error", Message = "Ação não permitida!" });

            var admin = user.GetEntity();
           
            if (repo.Update(admin, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário atualizado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria um usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do usuário criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]AdminUserModel user)
        {
            var admin = user.GetEntity();

            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null) 
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (CheckRoles(new string[] { "administrator", "partnerAdministrator" }))
            {
                admin.IdOperation = GetOperationId(out errorId);
                if (errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            if (CheckRole("partnerAdministrator"))
            {
                admin.IdOperationPartner = GetOperationPartnerId(out errorId);
                if(errorId != null)
                    return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });
            }

            if(!repo.CheckCPF(admin.Id, admin.Doc, out string errorCpf))
                return StatusCode(400, new JsonModel() { Status = "errorCPF", Message = errorCpf });
            if (!repo.CheckEmail(admin.Id, admin.Email, out string errorEmail))
                return StatusCode(400, new JsonModel() { Status = "errorEmail", Message = errorEmail });

            if (repo.Create(admin, idAdminUser, out string error))
            {
                var code = HttpUtility.UrlEncode(Helper.SecurityHelper.SimpleEncryption(admin.Email));
                string body = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {admin.Name}.</p><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Você foi cadastrado na plataforma <b>Sistema Rebens</b>, clique no botão <b>“Ativar Cadastro”</b> para validar seu cadastro e cadastrar sua senha.</p>";
                body += $"<br /><p style=\"text-align:center;\"><a href=\"{constant.URL}#/validate?c={code}\" target=\"_blank\" style=\"display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#08061e;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;\">ATIVAR CADASTRO</a></p>";

                var listDestinataries = new Dictionary<string, string>() { { admin.Email, admin.Name } };
                Helper.EmailHelper.SendAdminEmail(listDestinataries, "Rebens - Validação de cadastro", body, out error);


                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Usuário criado com sucesso!", Id = admin.Id });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um usuário
        /// </summary>
        /// <param name="id">Id do usuário a ser apagada</param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for excluido com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Delete(int id)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            if (repo.Delete(id, idAdminUser, out string error))
                return Ok(new JsonModel() { Status = "ok", Message = "Usuário apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Reenvia o email de validação do cliente
        /// </summary>
        /// <param name="id">id do usuário</param>
        /// <returns></returns>
        /// <response code="200">Se o e-mail for enviado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("ResendValidation/{id}")]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult ResendValidation(int id)
        {
            var admin = repo.Read(id, out string error);

            if (admin != null)
            {
                var code = HttpUtility.UrlEncode(Helper.SecurityHelper.SimpleEncryption(admin.Email));
                string body = $"<p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Olá, {admin.Name}.</p> <br /><p style='text-align:center; font-size: 14px; font-family:verdana, arial, Helvetica; color: #666666; margin: 0;padding: 0 20px;'>Clique no botão <b>“Alterar Senha”</b>  para cadastrar uma nova senha.</p>";
                body += $"<br /><br /><p style=\"text-align:center;\"><a href=\"{constant.AppSettings.App.URL}#/validate?c={code}\" target=\"_blank\" style=\"display:inline-block;margin:0;outline:none;text-align:center;text-decoration:none;padding: 15px 50px;background-color:#08061e;color:#ffffff;font-size: 14px; font-family:verdana, arial, Helvetica;border-radius:50px;\">ALTERAR SENHA</a></p>";
                var listDestinataries = new Dictionary<string, string>() { { admin.Email, admin.Name } };
                Helper.EmailHelper.SendAdminEmail(listDestinataries, "Rebens - Alteração de senha", body, out error);

                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "E-mail de validação reenviado com sucesso!" });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = "Usuário não encontrado!" });
        }

        /// <summary>
        /// Ativa/Inativa um usuário
        /// </summary>
        /// <param name="id">id do usuário</param>
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
    }
}