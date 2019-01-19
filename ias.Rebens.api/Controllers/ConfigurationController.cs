using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ias.Rebens.api.Models;

namespace ias.Rebens.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private IConfigurationRepository repo;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="configurationRepository">Injeção de dependencia do repositório de Configuraçõe</param>
        public ConfigurationController(IConfigurationRepository configurationRepository)
        {
            this.repo = configurationRepository;
        }

        /// <summary>
        /// Atualiza uma Configuração
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem</returns>
        /// <response code="200">Se o objeto for atualizado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPut]
        [ProducesResponseType(typeof(JsonModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Put([FromBody]ConfigurationModel configuration)
        {
            var model = new JsonModel();
            string error = null;

            var part = configuration.GetEntity();
            if (repo.Update(part, out error))
                return Ok(new JsonModel() { Status = "ok", Message = "Configuração atualizada com sucesso!" });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna a configuração conforme o Id da Operação
        /// </summary>
        /// <param name="id">Id da configuração desejada</param>
        /// <returns>configuração</returns>
        /// <response code="200">Retorna a configuração, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonDataModel<ConfigurationModel>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Get(int id)
        {
            var config = repo.Read(id, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (config == null || config.Id == 0)
                    return NoContent();

                return Ok(new JsonDataModel<ConfigurationModel>() { Data = new ConfigurationModel(config) });
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Cria uma configuração
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>Retorna um objeto com o status (ok, error), e uma mensagem, e o Id da categoria criada</returns>
        /// <response code="200">Se o objeto for criado com sucesso</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpPost]
        [ProducesResponseType(typeof(JsonCreateResultModel), 200)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult Post([FromBody] ConfigurationModel configuration)
        {
            var config = configuration.GetEntity();

            if (repo.Create(config, out string error))
                return Ok(new JsonCreateResultModel() { Status = "ok", Message = "Configuração criada com sucesso!", Id = config.Id });

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }

        /// <summary>
        /// Retorna uma lista de configuraçãoe
        /// </summary>
        /// <param name="idOperation">id da operação</param>
        /// <param name="type">Tipo da configuração</param>
        /// <returns>Lista com as configurações encontradas</returns>
        /// <response code="200">Retorna a lista, ou algum erro caso interno</response>
        /// <response code="204">Se não encontrar nada</response>
        /// <response code="400">Se ocorrer algum erro</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Helper.Config.Configuration>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        public IActionResult List([FromQuery]int idOperation, [FromQuery]int type)
        {
            var list = repo.ListConfiguration(idOperation, type, out string error);

            if (string.IsNullOrEmpty(error))
            {
                if (list == null || list.Count == 0)
                    return NoContent();

                return Ok(list);
            }

            return StatusCode(400, new JsonModel() { Status = "error", Message = error });
        }
    }
}