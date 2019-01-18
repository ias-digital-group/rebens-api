using System;
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
        /// Config info
        /// </summary>
        [Required]
        public string Config { get; set; }
    }
}
