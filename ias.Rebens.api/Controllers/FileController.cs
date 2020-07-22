using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ias.Rebens.api.Controllers
{
    /// <summary>
    /// Partner Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/File")]
    [ApiController]
    public class FileController : BaseApiController
    {
        private IFileRepository repo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="fileRepository"></param>
        public FileController(IFileRepository fileRepository)
        {
            this.repo = fileRepository;
        }

        /// <summary>
        /// Cria um arquivo
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, caso ok, retorna o id do objeto criado</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost, Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody]FileModel file)
        {
            int idAdminUser = GetAdminUserId(out string errorId);
            string adminUserName = GetAdminUserName(out _) + " " + GetAdminUserSurname(out _);
            if (errorId != null)
                return StatusCode(400, new JsonModel() { Status = "error", Message = errorId });

            var oFile = file.GetEntity();
            if (repo.Create(oFile, idAdminUser, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Arquivo criado com sucesso!", Id = oFile.Id, Extra = TimeZoneInfo.ConvertTimeFromUtc(oFile.Created, Constant.TimeZone).ToString("dd/MM/yyyy - HH:mm", Constant.FormatProvider) });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Apaga um arquivo
        /// </summary>
        /// <param name="id">Id do arquivo a ser apagado</param>
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
                return Ok(new JsonModel() { Status = "ok", Message = "Arquivo apagado com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Lista todos os arquivos de um item
        /// </summary>
        /// <param name="idItem">Id do Item vinculado ao arquivo</param>
        /// <param name="itemType">
        /// Tipo do item vinculado ao arquivo, tipos:
        /// - Benefício = 4,
        /// - Faculdadae = 9,
        /// - Curso = 13,
        /// - Cursos livres = 19,
        /// - Operação = 22,
        /// - Parceiros = 23,
        /// - Raspadinha = 26,
        /// - Prêmio da Raspadinha = 27
        /// </param>
        /// <returns>Lista com os arquivos encontrados</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{idItem}/{itemType}"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
        [ProducesResponseType(typeof(JsonDataModel<List<FileModel>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List(int idItem, int itemType)
        {
            var list = repo.List(idItem, itemType, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                var ret = new JsonDataModel<List<FileModel>>
                {
                    Data = new List<FileModel>()
                };

                foreach (var file in list)
                    ret.Data.Add(new FileModel(file));

                return Ok(ret);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}
