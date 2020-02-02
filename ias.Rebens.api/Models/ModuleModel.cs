using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class ModuleModel
    {
        /// <summary>
        /// Id do módulo
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome do Módulo
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Information
        /// </summary>
        [Required]
        public string Information { get; set; }


        public bool Checked { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public ModuleModel() { }

        /// <summary>
        /// Construtor que recebe um objeto Module e popula os atributos
        /// </summary>
        /// <param name="Module"></param>
        public ModuleModel(Module module) {
            if (module != null)
            {
                this.Id = module.Id;
                this.Name = module.Name;
                this.Information = module.Information;
                this.Checked = false;
            }
        }
    }
}
