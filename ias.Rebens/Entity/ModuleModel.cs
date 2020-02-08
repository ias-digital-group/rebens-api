using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public class ModuleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Checked { get; set; }
        public Helper.Config.ModuleInformation Info { get; set; }

        public ModuleModel() { }
        
        public ModuleModel(Module module) {
            this.Id = module.Id;
            this.Name = module.Name;
            this.Title = module.Title;
            this.Info = Helper.Config.JsonHelper<Helper.Config.ModuleInformation>.GetObject(module.Information);
            this.Created = module.Created;
            this.Modified = module.Modified;
        }
    }
}
