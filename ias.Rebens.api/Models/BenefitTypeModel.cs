using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    public class BenefitTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public BenefitTypeModel() { }
        public BenefitTypeModel(BenefitType type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
