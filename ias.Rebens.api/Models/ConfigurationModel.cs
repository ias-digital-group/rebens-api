using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// Id Operation
        /// </summary>
        [Required]
        public int IdOperation { get; set; }
        /// <summary>
        /// Configuration Type
        /// </summary>
        [Required]
        public int ConfigurationType { get; set; }
        /// <summary>
        /// Configurations
        /// </summary>
        [Required]
        public List<Helper.Config.Configuration> Configurations { get; set; }
        /// <summary>
        /// Construtror
        /// </summary>
        public ConfigurationModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Configuration e já popula os atributos
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigurationModel(Configuration configuration)
        {
            this.Id = configuration.Id;
            this.IdOperation = configuration.IdOperation;
            this.ConfigurationType = configuration.ConfigurationType;
            this.Configurations = Helper.Config.ConfigurationHelper.GetConfigurations(configuration.Config);
        }

        /// <summary>
        /// Retorna um objeto Configuration com as informações
        /// </summary>
        /// <returns></returns>
        public Configuration GetEntity()
        {
            return new Configuration()
            {
                Id = this.Id,
                IdOperation = this.IdOperation,
                ConfigurationType = this.ConfigurationType,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Config = Helper.Config.ConfigurationHelper.GetConfigurationString(this.Configurations)
            };
        }
    }

    /// <summary>
    /// Tipos de configuração
    /// </summary>
    public class ConfigurationTypeModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }
    }
}
